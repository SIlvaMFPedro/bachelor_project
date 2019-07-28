using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text; 


[RequireComponent(typeof(Animator))]
public class AvatarController : MonoBehaviour
{	
	// Bool that has the characters (facing the player) actions become mirrored. Default false.
	public bool mirroredMovement = false;
	
	// Bool that determines whether the avatar is allowed to move in vertical direction.
	public bool verticalMovement = false;
	
	// Rate at which avatar will move through the scene. The rate multiplies the movement speed (.001f, i.e dividing by 1000, unity's framerate).
	protected int moveRate = 1;
	
	// Slerp smooth factor
	public float smoothFactor = 5f;
	
	// Whether the offset node must be repositioned to the user's coordinates, as reported by the sensor or not.
	public bool offsetRelativeToSensor = false;

    // Allow input of a material colors in a set sized array
    public Material[] materials;
    // 3d object that we want to render;
    public Renderer[] renderers; //we are rendering the joints of the avatar;
	

	// The body root node
	protected Transform bodyRoot;
	
	// A required variable if you want to rotate the model in space.
	protected GameObject offsetNode;
	
	// Variable to hold all them bones. It will initialize the same size as initialRotations.
	protected Transform[] bones;
	
	// Rotations of the bones when the Kinect tracking starts.
	protected Quaternion[] initialRotations;
	protected Quaternion[] initialLocalRotations;
	
	// Initial position and rotation of the transform
	protected Vector3 initialPosition;
	protected Quaternion initialRotation;
    protected Vector3 initialJointPos;
    protected Quaternion initialJointRot;
	
	// Calibration Offset Variables for Character Position.
	protected bool offsetCalibrated = false;
	protected float xOffset, yOffset, zOffset;

	// private instance of the KinectManager
	protected KinectManager kinectManager;
    protected uint playerID;

	// transform caching gives performance boost since Unity calls GetComponent<Transform>() each time you call transform 
	private Transform _transformCache;

    private int index = 1; //index of color materials;

	public new Transform transform
	{
		get
		{
			if (!_transformCache) 
				_transformCache = base.transform;
			
			return _transformCache;
		}
	}

    public void Awake()
    {
        // check for double start
        if (bones != null)
            return;

        // inits the bones array
        bones = new Transform[22];

        // Initial rotations and directions of the bones.
        initialRotations = new Quaternion[bones.Length];
        initialLocalRotations = new Quaternion[bones.Length];

        // Map bones to the points the Kinect tracks
        MapBones();

        // Get initial bone rotations
        GetInitialRotations();

        // get the initial renderer of the avatar;
        int i;
        for (i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = true; //makes the renderered 3d object visable if enabled;
        }
    }
	
	// Update the avatar each frame.
    public void UpdateAvatar(uint UserID)
    {	
		if(!transform.gameObject.activeInHierarchy) 
			return;
		
		// Get the KinectManager instance
		if(kinectManager == null)
		{
			kinectManager = KinectManager.Instance;
		}
        //get 1st player;
        playerID = kinectManager != null ? kinectManager.GetPlayer1ID() : 0;

        if (playerID <= 0)
        {
            // reset the pointman position and rotation
            if (transform.position != initialPosition)
            {
                transform.position = initialPosition;
            }

            if (transform.rotation != initialRotation)
            {
                transform.rotation = initialRotation;
            }

            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].gameObject.SetActive(true);

                bones[i].transform.localPosition = Vector3.zero;
                bones[i].transform.localRotation = Quaternion.identity;
            }

            return;
        }

        // move the avatar to its Kinect position
        MoveAvatar(UserID);

		for (var boneIndex = 0; boneIndex < bones.Length; boneIndex++)
		{
			if (!bones[boneIndex]) 
				continue;
			
			if(boneIndex2JointMap.ContainsKey(boneIndex))
			{
				KinectWrapper.NuiSkeletonPositionIndex joint = !mirroredMovement ? boneIndex2JointMap[boneIndex] : boneIndex2MirrorJointMap[boneIndex];
				TransformBone(UserID, joint, boneIndex, !mirroredMovement);
			}
			else if(specIndex2JointMap.ContainsKey(boneIndex))
			{
				// special bones (clavicles)
				List<KinectWrapper.NuiSkeletonPositionIndex> alJoints = !mirroredMovement ? specIndex2JointMap[boneIndex] : specIndex2MirrorJointMap[boneIndex];
				
				if(alJoints.Count >= 2)
				{
					//Vector3 baseDir = alJoints[0].ToString().EndsWith("Left") ? Vector3.left : Vector3.right;
					//TransformSpecialBone(UserID, alJoints[0], alJoints[1], boneIndex, baseDir, !mirroredMovement);
				}
			}
		}
	}
	
	// Set bones to their initial positions and rotations
	public void ResetToInitialPosition()
	{	
		if(bones == null)
			return;
		
		if(offsetNode != null)
		{
			offsetNode.transform.rotation = Quaternion.identity;
		}
		else
		{
			transform.rotation = Quaternion.identity;
		}
		
		// For each bone that was defined, reset to initial position.
		for (int i = 0; i < bones.Length; i++)
		{
			if (bones[i] != null)
			{
				bones[i].rotation = initialRotations[i];
			}
		}
		
		if(bodyRoot != null)
		{
			bodyRoot.localPosition = Vector3.zero;
			bodyRoot.localRotation = Quaternion.identity;
		}
		
		// Restore the offset's position and rotation
		if(offsetNode != null)
		{
			offsetNode.transform.position = initialPosition;
			offsetNode.transform.rotation = initialRotation;
		}
		else
		{
			transform.position = initialPosition;
			transform.rotation = initialRotation;
		}
	}
	
	// Invoked on the successful calibration of a player.
	public void SuccessfulCalibration(uint userId)
	{
		// reset the models position
		if(offsetNode != null)
		{
			offsetNode.transform.rotation = initialRotation;
		}
		
		// re-calibrate the position offset
		offsetCalibrated = false;
	}
	
	// Apply the rotations tracked by kinect to the joints.
	protected void TransformBone(uint userId, KinectWrapper.NuiSkeletonPositionIndex joint, int boneIndex, bool flip)
    {
		Transform boneTransform = bones[boneIndex];
		if(boneTransform == null || kinectManager == null)
			return;
		
		int iJoint = (int)joint;
		if(iJoint < 0)
			return;
		
		// Get Kinect joint orientation
		Quaternion jointRotation = kinectManager.GetJointOrientation(userId, iJoint, flip);
		if(jointRotation == Quaternion.identity)
			return;
		
		// Smoothly transition to the new rotation
		Quaternion newRotation = Kinect2AvatarRot(jointRotation, boneIndex);
		
		if(smoothFactor != 0f)
        	boneTransform.rotation = Quaternion.Slerp(boneTransform.rotation, newRotation, smoothFactor * Time.deltaTime);
		else
			boneTransform.rotation = newRotation;
	}
	
	// Apply the rotations tracked by kinect to a special joint
	protected void TransformSpecialBone(uint userId, KinectWrapper.NuiSkeletonPositionIndex joint, KinectWrapper.NuiSkeletonPositionIndex jointParent, int boneIndex, Vector3 baseDir, bool flip)
	{
		Transform boneTransform = bones[boneIndex];
		if(boneTransform == null || kinectManager == null)
			return;
		
		if(!kinectManager.IsJointTracked(userId, (int)joint) || 
		   !kinectManager.IsJointTracked(userId, (int)jointParent))
		{
			return;
		}
		
		Vector3 jointDir = kinectManager.GetDirectionBetweenJoints(userId, (int)jointParent, (int)joint, false, true);
		Quaternion jointRotation = jointDir != Vector3.zero ? Quaternion.FromToRotation(baseDir, jointDir) : Quaternion.identity;
		
//		if(!flip)
//		{
//			Vector3 mirroredAngles = jointRotation.eulerAngles;
//			mirroredAngles.y = -mirroredAngles.y;
//			mirroredAngles.z = -mirroredAngles.z;
//			
//			jointRotation = Quaternion.Euler(mirroredAngles);
//		}
		
		if(jointRotation != Quaternion.identity)
		{
			// Smoothly transition to the new rotation
			Quaternion newRotation = Kinect2AvatarRot(jointRotation, boneIndex);
			
			if(smoothFactor != 0f)
				boneTransform.rotation = Quaternion.Slerp(boneTransform.rotation, newRotation, smoothFactor * Time.deltaTime);
			else
				boneTransform.rotation = newRotation;
		}
		
	}
	
	// Moves the avatar in 3D space - pulls the tracked position of the spine and applies it to root.
	// Only pulls positional, not rotational.
	protected void MoveAvatar(uint UserID)
	{
		if(bodyRoot == null || kinectManager == null)
			return;
		if(!kinectManager.IsJointTracked(UserID, (int)KinectWrapper.NuiSkeletonPositionIndex.HipCenter))
			return;
		
        // Get the position of the body and store it.
		Vector3 trans = kinectManager.GetUserPosition(UserID);

        // If this is the first time we're moving the avatar, set the offset. Otherwise ignore it.
        if (!offsetCalibrated)
		{
			offsetCalibrated = true;
			
			xOffset = !mirroredMovement ? trans.x * moveRate : -trans.x * moveRate;
			yOffset = trans.y * moveRate;
			zOffset = -trans.z * moveRate;
			
			if(offsetRelativeToSensor)
			{
				Vector3 cameraPos = Camera.main.transform.position;
				
				float yRelToAvatar = (offsetNode != null ? offsetNode.transform.position.y : transform.position.y) - cameraPos.y;
				Vector3 relativePos = new Vector3(trans.x * moveRate, yRelToAvatar, trans.z * moveRate);
				Vector3 offsetPos = cameraPos + relativePos;
				
				if(offsetNode != null)
				{
					offsetNode.transform.position = offsetPos;
				}
				else
				{
					transform.position = offsetPos;
				}
			}
		}
	
		// Smoothly transition to the new position
		Vector3 targetPos = Kinect2AvatarPos(trans, verticalMovement);
        changeColorOfAvatar(bones);

        //update the position of the bones;
        for(int i = 0; i < bones.Length; i++)
        {
            if(bones[i] != null)
            {
                int joint = mirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(i) : i;
                if(kinectManager.IsJointTracked(playerID, joint))
                {
                    bones[i].gameObject.SetActive(true);
                    initialJointPos = kinectManager.GetJointPosition(playerID, joint);
                    initialJointRot = kinectManager.GetJointOrientation(playerID, joint, !mirroredMovement);
                    if(materials.Length == 0)
                    {
                        return;
                    }
                    if(targetPos == initialJointPos)
                    {
                        return; //change color of mesh to green;
                    }
                    else
                    {
                        return; //change color of mesh to red;
                    }
                }
                else
                {
                    bones[i].gameObject.SetActive(false);
                }
            }

        }
		if(smoothFactor != 0f)
			bodyRoot.localPosition = Vector3.Lerp(bodyRoot.localPosition, targetPos, smoothFactor * Time.deltaTime);
		else
			bodyRoot.localPosition = targetPos;
	}
	
	// If the bones to be mapped have been declared, map that bone to the model.
	protected virtual void MapBones()
	{
		// make OffsetNode as a parent of model transform.
		offsetNode = new GameObject(name + "Ctrl") { layer = transform.gameObject.layer, tag = transform.gameObject.tag };
		offsetNode.transform.position = transform.position;
		offsetNode.transform.rotation = transform.rotation;
		offsetNode.transform.parent = transform.parent;
		
		transform.parent = offsetNode.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		
		// take model transform as body root
		bodyRoot = transform;
		
		// get bone transforms from the animator component
		var animatorComponent = GetComponent<Animator>();
		
		for (int boneIndex = 0; boneIndex < bones.Length; boneIndex++)
		{
			if (!boneIndex2MecanimMap.ContainsKey(boneIndex)) 
				continue;
			
			bones[boneIndex] = animatorComponent.GetBoneTransform(boneIndex2MecanimMap[boneIndex]);
		}
	}
	
	// Capture the initial rotations of the bones
	protected void GetInitialRotations()
	{
		// save the initial rotation
		if(offsetNode != null)
		{
			initialPosition = offsetNode.transform.position;
			initialRotation = offsetNode.transform.rotation;
			
			offsetNode.transform.rotation = Quaternion.identity;
		}
		else
		{
			initialPosition = transform.position;
			initialRotation = transform.rotation;
			
			transform.rotation = Quaternion.identity;
		}
		
		for (int i = 0; i < bones.Length; i++)
		{
			if (bones[i] != null)
			{
				initialRotations[i] = bones[i].rotation; // * Quaternion.Inverse(initialRotation);
				initialLocalRotations[i] = bones[i].localRotation;
			}
		}
		
		// Restore the initial rotation
		if(offsetNode != null)
		{
			offsetNode.transform.rotation = initialRotation;
		}
		else
		{
			transform.rotation = initialRotation;
		}
	}
	
	// Converts kinect joint rotation to avatar joint rotation, depending on joint initial rotation and offset rotation
	protected Quaternion Kinect2AvatarRot(Quaternion jointRotation, int boneIndex)
	{
		// Apply the new rotation.
        Quaternion newRotation = jointRotation * initialRotations[boneIndex];
		
		//If an offset node is specified, combine the transform with its
		//orientation to essentially make the skeleton relative to the node
		if (offsetNode != null)
		{
			// Grab the total rotation by adding the Euler and offset's Euler.
			Vector3 totalRotation = newRotation.eulerAngles + offsetNode.transform.rotation.eulerAngles;
			// Grab our new rotation.
			newRotation = Quaternion.Euler(totalRotation);
		}
		
		return newRotation;
	}
	
	// Converts Kinect position to avatar skeleton position, depending on initial position, mirroring and move rate
	protected Vector3 Kinect2AvatarPos(Vector3 jointPosition, bool bMoveVertically)
	{
		float xPos;
		float yPos;
		float zPos;
		
		// If movement is mirrored, reverse it.
		if(!mirroredMovement)
			xPos = jointPosition.x * moveRate - xOffset;
		else
			xPos = -jointPosition.x * moveRate - xOffset;
		
		yPos = jointPosition.y * moveRate - yOffset;
		zPos = -jointPosition.z * moveRate - zOffset;
		
		// If we are tracking vertical movement, update the y. Otherwise leave it alone.
		Vector3 avatarJointPos = new Vector3(xPos, bMoveVertically ? yPos : 0f, zPos);
		
		return avatarJointPos;
	}
    public void changeColorOfAvatar(Transform[] bones)
    {
        //update the color of the bones;
        for (int i = 0; i < bones.Length; i++)
        {
            if (bones[i] != null)
            {
                int hips_joint = mirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(0) : 0;
                int spine_joint = mirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(1) : 1;
                int neck_joint = mirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(2) : 2;
                int head_joint = mirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(3) : 3;
                int left_shoulder_joint = mirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(4) : 4;
                int left_upperarm_joint = mirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(5) : 5;
                int left_elbow_joint = mirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(6) : 6;
                int left_hand_joint = mirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(7) : 7;
                int right_shoulder_joint = mirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(9) : 9;
                int right_upperarm_joint = mirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(10) : 10;
                int right_elbow_joint = mirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(11) : 11;
                int right_hand_joint = mirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(12) : 12;
                int left_upperleg_joint = mirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(14) : 14;
                int left_knee_joint = mirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(15) : 15;
                int left_foot_joint = mirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(16) : 16;
                int right_upperleg_joint = mirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(18) : 18;
                int right_knee_joint = mirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(19) : 19;
                int right_foot_joint = mirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(20) : 20;

                if (kinectManager.IsJointTracked(playerID, hips_joint))
                {
                    bones[0].gameObject.SetActive(true);
                    initialJointPos = kinectManager.GetJointPosition(playerID, hips_joint);
                    initialJointRot = kinectManager.GetJointOrientation(playerID, hips_joint, !mirroredMovement);
                    if (materials.Length == 0)
                    {
                        return;
                    }
                    if ((bones[0].transform.position != initialJointPos) || (bones[0].transform.rotation != initialJointRot))
                    {

                        Material mat = Instantiate(Resources.Load("body_perna3", typeof(Material))) as Material;
                        //Material[] mats = new Material[] { mat };
                        renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;


                    }
                    
                    else
                    {

                        Material mat = Instantiate(Resources.Load("No Name", typeof(Material))) as Material;
                        //Material[] mats = new Material[] { mat };
                        renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;


                    }
                    
                }
                if (kinectManager.IsJointTracked(playerID, spine_joint))
                {
                    bones[1].gameObject.SetActive(true);
                    initialJointPos = kinectManager.GetJointPosition(playerID, spine_joint);
                    initialJointRot = kinectManager.GetJointOrientation(playerID, spine_joint, !mirroredMovement);
                    if ((bones[1].transform.position != initialJointPos) || (bones[1].transform.rotation != initialJointRot))
                    {

                        Material mat = Instantiate(Resources.Load("body", typeof(Material))) as Material;
                        //Material[] mats = new Material[] { mat };
                        renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;


                    }
                    
                    else
                    {

                        Material mat = Instantiate(Resources.Load("No Name", typeof(Material))) as Material;
                        //Material[] mats = new Material[] { mat };
                        renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;

                    }
                    
                }
                if (kinectManager.IsJointTracked(playerID, neck_joint))
                {
                    bones[2].gameObject.SetActive(true);
                    initialJointPos = kinectManager.GetJointPosition(playerID, neck_joint);
                    initialJointRot = kinectManager.GetJointOrientation(playerID, neck_joint, !mirroredMovement);
                    if ((bones[2].transform.position != initialJointPos) || (bones[2].transform.rotation != initialJointRot))
                    {

                        Material mat = Instantiate(Resources.Load("head_neck", typeof(Material))) as Material;
                        //Material[] mats = new Material[] { mat };
                        renderers[0].GetComponent<SkinnedMeshRenderer>().material = mat;


                    }
                    
                    else
                    {

                        Material mat = Instantiate(Resources.Load("No Name", typeof(Material))) as Material;
                        //Material[] mats = new Material[] { mat };
                        renderers[0].GetComponent<SkinnedMeshRenderer>().material = mat;

                    }
                    
                }
                if (kinectManager.IsJointTracked(playerID, head_joint))
                {
                    bones[3].gameObject.SetActive(true);
                    initialJointPos = kinectManager.GetJointPosition(playerID, head_joint);
                    initialJointRot = kinectManager.GetJointOrientation(playerID, head_joint, !mirroredMovement);
                    if ((bones[3].transform.position != initialJointPos) || (bones[3].transform.rotation != initialJointRot))
                    {

                        Material mat = Instantiate(Resources.Load("head", typeof(Material))) as Material;
                        //Material[] mats = new Material[] { mat };
                        renderers[0].GetComponent<SkinnedMeshRenderer>().material = mat;


                    }
                    
                    else
                    {

                        Material mat = Instantiate(Resources.Load("No Name", typeof(Material))) as Material;
                        //Material[] mats = new Material[] { mat };
                        renderers[0].GetComponent<SkinnedMeshRenderer>().material = mat;

                    }
                    
                }
                if (kinectManager.IsJointTracked(playerID, left_shoulder_joint))
                {
                    bones[4].gameObject.SetActive(true);
                    initialJointPos = kinectManager.GetJointPosition(playerID, left_shoulder_joint);
                    initialJointRot = kinectManager.GetJointOrientation(playerID, left_shoulder_joint, !mirroredMovement);
                    if ((bones[4].transform.position != initialJointPos) || (bones[4].transform.rotation != initialJointRot))
                    {

                        Material mat = Instantiate(Resources.Load("body_1_l", typeof(Material))) as Material;
                        //Material[] mats = new Material[] { mat };
                        renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;

                    }
                    
                    else
                    {

                        Material mat = Instantiate(Resources.Load("No Name", typeof(Material))) as Material;
                        //Material[] mats = new Material[] { mat };
                        renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;


                    }
                    
                 }
                 if (kinectManager.IsJointTracked(playerID, left_upperarm_joint))
                 {
                    bones[5].gameObject.SetActive(true);
                    initialJointPos = kinectManager.GetJointPosition(playerID, left_upperarm_joint);
                    initialJointRot = kinectManager.GetJointOrientation(playerID, left_upperarm_joint, !mirroredMovement);
                    if ((bones[5].transform.position != initialJointPos) || (bones[5].transform.rotation != initialJointRot))
                    {

                        Material mat = Instantiate(Resources.Load("body_1_l", typeof(Material))) as Material;
                        //Material[] mats = new Material[] { mat };
                        renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;

                    }
                        
                    else
                    {

                        Material mat = Instantiate(Resources.Load("No Name", typeof(Material))) as Material;
                        //Material[] mats = new Material[] { mat };
                        renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;

                    }
                        
                  }
                  if (kinectManager.IsJointTracked(playerID, left_elbow_joint))
                  {
                        bones[6].gameObject.SetActive(true);
                        initialJointPos = kinectManager.GetJointPosition(playerID, left_elbow_joint);
                        initialJointRot = kinectManager.GetJointOrientation(playerID, left_elbow_joint, !mirroredMovement);
                        if ((bones[6].transform.position != initialJointPos) || (bones[6].transform.rotation != initialJointRot))
                        {

                            Material mat = Instantiate(Resources.Load("body_0_l", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;

                        }
                        
                        else
                        {

                            Material mat = Instantiate(Resources.Load("No Name", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;

                        }
                        
                    }
                    if (kinectManager.IsJointTracked(playerID, left_hand_joint))
                    {
                        bones[7].gameObject.SetActive(true);
                        initialJointPos = kinectManager.GetJointPosition(playerID, left_hand_joint);
                        initialJointRot = kinectManager.GetJointOrientation(playerID, left_hand_joint, !mirroredMovement);
                        if ((bones[7].transform.position != initialJointPos) || (bones[7].transform.rotation != initialJointRot))
                        {

                            Material mat = Instantiate(Resources.Load("hands_left", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[2].GetComponent<SkinnedMeshRenderer>().material = mat;

                        }
                        
                        else
                        {

                            Material mat = Instantiate(Resources.Load("No Name", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[2].GetComponent<SkinnedMeshRenderer>().material = mat;

                        }
                        
                    }
                    if (kinectManager.IsJointTracked(playerID, right_shoulder_joint))
                    {
                        bones[9].gameObject.SetActive(true);
                        initialJointPos = kinectManager.GetJointPosition(playerID, right_shoulder_joint);
                        initialJointRot = kinectManager.GetJointOrientation(playerID, right_shoulder_joint, !mirroredMovement);
                        if ((bones[9].transform.position != initialJointPos) || (bones[9].transform.rotation != initialJointRot))
                        {

                            Material mat = Instantiate(Resources.Load("body_1_r", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;
                        }
                        
                        else
                        {

                            Material mat = Instantiate(Resources.Load("No Name", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;

                        }
                        
                    }
                    if (kinectManager.IsJointTracked(playerID, right_upperarm_joint))
                    {
                        bones[10].gameObject.SetActive(true);
                        initialJointPos = kinectManager.GetJointPosition(playerID, right_upperarm_joint);
                        initialJointRot = kinectManager.GetJointOrientation(playerID, right_upperarm_joint, !mirroredMovement);
                        if ((bones[10].transform.position != initialJointPos) || (bones[10].transform.rotation != initialJointRot))
                        {

                            Material mat = Instantiate(Resources.Load("body_1_r", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;

                        }
                        
                        else
                        {

                            Material mat = Instantiate(Resources.Load("No Name", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;

                        }
                        
                    }
                    if (kinectManager.IsJointTracked(playerID, right_elbow_joint))
                    {
                        bones[11].gameObject.SetActive(true);
                        initialJointPos = kinectManager.GetJointPosition(playerID, right_elbow_joint);
                        initialJointRot = kinectManager.GetJointOrientation(playerID, right_elbow_joint, !mirroredMovement);
                        if ((bones[11].transform.position != initialJointPos) || (bones[11].transform.rotation != initialJointRot))
                        {

                            Material mat = Instantiate(Resources.Load("body_0_r", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;


                        }
                        
                        else
                        {

                            Material mat = Instantiate(Resources.Load("No Name", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;

                        }
                        
                    }
                    if (kinectManager.IsJointTracked(playerID, right_hand_joint))
                    {
                        bones[12].gameObject.SetActive(true);
                        initialJointPos = kinectManager.GetJointPosition(playerID, right_hand_joint);
                        initialJointRot = kinectManager.GetJointOrientation(playerID, right_hand_joint, !mirroredMovement);
                        if ((bones[12].transform.position != initialJointPos) || (bones[12].transform.rotation != initialJointRot))
                        {

                            Material mat = Instantiate(Resources.Load("hands_right", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[2].GetComponent<SkinnedMeshRenderer>().material = mat;


                        }
                        
                        else
                        {

                            Material mat = Instantiate(Resources.Load("No Name", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[2].GetComponent<SkinnedMeshRenderer>().material = mat;

                        }
                        
                    }
                    if (kinectManager.IsJointTracked(playerID, left_upperleg_joint))
                    {
                        bones[14].gameObject.SetActive(true);
                        initialJointPos = kinectManager.GetJointPosition(playerID, left_upperleg_joint);
                        initialJointRot = kinectManager.GetJointOrientation(playerID, left_upperleg_joint, !mirroredMovement);
                        if ((bones[14].transform.position != initialJointPos) || (bones[14].transform.rotation != initialJointRot))
                        {

                            Material mat = Instantiate(Resources.Load("body_perna1", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;

                        }
                        
                        else
                        {

                            Material mat = Instantiate(Resources.Load("No Name", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;
                        }
                        
                    }
                    if (kinectManager.IsJointTracked(playerID, left_knee_joint))
                    {
                        bones[15].gameObject.SetActive(true);
                        initialJointPos = kinectManager.GetJointPosition(playerID, left_knee_joint);
                        initialJointRot = kinectManager.GetJointOrientation(playerID, left_knee_joint, !mirroredMovement);
                        if ((bones[15].transform.position != initialJointPos) || (bones[15].transform.rotation != initialJointRot))
                        {
                            Material mat = Instantiate(Resources.Load("body_perna1", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;

                        }
                        
                        else
                        {

                            Material mat = Instantiate(Resources.Load("No Name", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;

                        }
                        
                    }
                    if (kinectManager.IsJointTracked(playerID, left_foot_joint))
                    {
                        bones[16].gameObject.SetActive(true);
                        initialJointPos = kinectManager.GetJointPosition(playerID, left_foot_joint);
                        initialJointRot = kinectManager.GetJointOrientation(playerID, left_foot_joint, !mirroredMovement);
                        if ((bones[16].transform.position != initialJointPos) || (bones[16].transform.rotation != initialJointRot))
                        {

                            Material mat = Instantiate(Resources.Load("Red Material", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[3].GetComponent<SkinnedMeshRenderer>().material = mat;


                        }
                        
                        else
                        {

                            Material mat = Instantiate(Resources.Load("No Name", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[3].GetComponent<SkinnedMeshRenderer>().material = mat;

                        }
                        
                    }
                    if (kinectManager.IsJointTracked(playerID, right_upperleg_joint))
                    {
                        bones[18].gameObject.SetActive(true);
                        initialJointPos = kinectManager.GetJointPosition(playerID, right_upperleg_joint);
                        initialJointRot = kinectManager.GetJointOrientation(playerID, right_upperleg_joint, !mirroredMovement);
                        if ((bones[18].transform.position != initialJointPos) || (bones[18].transform.rotation != initialJointRot))
                        {

                            Material mat = Instantiate(Resources.Load("body_perna2", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;

                        }
                        
                        else
                        {

                            Material mat = Instantiate(Resources.Load("No Name", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;

                        }
                        
                    }
                    if (kinectManager.IsJointTracked(playerID, right_knee_joint))
                    {
                        bones[19].gameObject.SetActive(true);
                        initialJointPos = kinectManager.GetJointPosition(playerID, right_knee_joint);
                        initialJointRot = kinectManager.GetJointOrientation(playerID, right_knee_joint, !mirroredMovement);
                        if ((bones[19].transform.position != initialJointPos) || (bones[19].transform.rotation != initialJointRot))
                        {

                            Material mat = Instantiate(Resources.Load("body_perna2", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;

                        }
                        
                        else
                        {

                            Material mat = Instantiate(Resources.Load("No Name", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[1].GetComponent<SkinnedMeshRenderer>().material = mat;
                        }
                        
                    }
                    if (kinectManager.IsJointTracked(playerID, right_foot_joint))
                    {
                        bones[20].gameObject.SetActive(true);
                        initialJointPos = kinectManager.GetJointPosition(playerID, right_foot_joint);
                        initialJointRot = kinectManager.GetJointOrientation(playerID, right_foot_joint, !mirroredMovement);
                        if ((bones[20].transform.position != initialJointPos) || (bones[20].transform.rotation != initialJointRot))
                        {

                            Material mat = Instantiate(Resources.Load("Red Material", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[3].GetComponent<SkinnedMeshRenderer>().material = mat;

                        }
                        
                        else
                        {

                            Material mat = Instantiate(Resources.Load("No Name", typeof(Material))) as Material;
                            //Material[] mats = new Material[] { mat };
                            renderers[3].GetComponent<SkinnedMeshRenderer>().material = mat;

                        }
                        
                    }
                    else
                    {
                        bones[0].gameObject.SetActive(false);
                        bones[1].gameObject.SetActive(false);
                        bones[2].gameObject.SetActive(false);
                        bones[3].gameObject.SetActive(false);
                        bones[4].gameObject.SetActive(false);
                        bones[5].gameObject.SetActive(false);
                        bones[6].gameObject.SetActive(false);
                        bones[7].gameObject.SetActive(false);
                        bones[9].gameObject.SetActive(false);
                        bones[10].gameObject.SetActive(false);
                        bones[11].gameObject.SetActive(false);
                        bones[12].gameObject.SetActive(false);
                        bones[14].gameObject.SetActive(false);
                        bones[15].gameObject.SetActive(false);
                        bones[16].gameObject.SetActive(false);
                        bones[18].gameObject.SetActive(false);
                        bones[19].gameObject.SetActive(false);
                        bones[20].gameObject.SetActive(false);
                   }
            }
        }
    }
    
	
	// dictionaries to speed up bones' processing
	// the author of the terrific idea for kinect-joints to mecanim-bones mapping
	// along with its initial implementation, including following dictionary is
	// Mikhail Korchun (korchoon@gmail.com). Big thanks to this guy!
	private readonly Dictionary<int, HumanBodyBones> boneIndex2MecanimMap = new Dictionary<int, HumanBodyBones>
	{
		{0, HumanBodyBones.Hips},
		{1, HumanBodyBones.Spine},
		{2, HumanBodyBones.Neck},
		{3, HumanBodyBones.Head},
		
		{4, HumanBodyBones.LeftShoulder},
		{5, HumanBodyBones.LeftUpperArm},
		{6, HumanBodyBones.LeftLowerArm},
		{7, HumanBodyBones.LeftHand},
		{8, HumanBodyBones.LeftIndexProximal},

		{9, HumanBodyBones.RightShoulder},
		{10, HumanBodyBones.RightUpperArm},
		{11, HumanBodyBones.RightLowerArm},
		{12, HumanBodyBones.RightHand},
		{13, HumanBodyBones.RightIndexProximal},

		{14, HumanBodyBones.LeftUpperLeg},
		{15, HumanBodyBones.LeftLowerLeg},
		{16, HumanBodyBones.LeftFoot},
		{17, HumanBodyBones.LeftToes},
		
		{18, HumanBodyBones.RightUpperLeg},
		{19, HumanBodyBones.RightLowerLeg},
		{20, HumanBodyBones.RightFoot},
		{21, HumanBodyBones.RightToes},
	};
	
	protected readonly Dictionary<int, KinectWrapper.NuiSkeletonPositionIndex> boneIndex2JointMap = new Dictionary<int, KinectWrapper.NuiSkeletonPositionIndex>
	{
		{0, KinectWrapper.NuiSkeletonPositionIndex.HipCenter},
		{1, KinectWrapper.NuiSkeletonPositionIndex.Spine},
		{2, KinectWrapper.NuiSkeletonPositionIndex.ShoulderCenter},
		{3, KinectWrapper.NuiSkeletonPositionIndex.Head},
		
		{5, KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft},
		{6, KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft},
		{7, KinectWrapper.NuiSkeletonPositionIndex.WristLeft},
		{8, KinectWrapper.NuiSkeletonPositionIndex.HandLeft},
		
		{10, KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight},
		{11, KinectWrapper.NuiSkeletonPositionIndex.ElbowRight},
		{12, KinectWrapper.NuiSkeletonPositionIndex.WristRight},
		{13, KinectWrapper.NuiSkeletonPositionIndex.HandRight},
		
		{14, KinectWrapper.NuiSkeletonPositionIndex.HipLeft},
		{15, KinectWrapper.NuiSkeletonPositionIndex.KneeLeft},
		{16, KinectWrapper.NuiSkeletonPositionIndex.AnkleLeft},
		{17, KinectWrapper.NuiSkeletonPositionIndex.FootLeft},
		
		{18, KinectWrapper.NuiSkeletonPositionIndex.HipRight},
		{19, KinectWrapper.NuiSkeletonPositionIndex.KneeRight},
		{20, KinectWrapper.NuiSkeletonPositionIndex.AnkleRight},
		{21, KinectWrapper.NuiSkeletonPositionIndex.FootRight},
	};
	
	protected readonly Dictionary<int, List<KinectWrapper.NuiSkeletonPositionIndex>> specIndex2JointMap = new Dictionary<int, List<KinectWrapper.NuiSkeletonPositionIndex>>
	{
		{4, new List<KinectWrapper.NuiSkeletonPositionIndex> {KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft, KinectWrapper.NuiSkeletonPositionIndex.ShoulderCenter} },
		{9, new List<KinectWrapper.NuiSkeletonPositionIndex> {KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight, KinectWrapper.NuiSkeletonPositionIndex.ShoulderCenter} },
	};
	
	protected readonly Dictionary<int, KinectWrapper.NuiSkeletonPositionIndex> boneIndex2MirrorJointMap = new Dictionary<int, KinectWrapper.NuiSkeletonPositionIndex>
	{
		{0, KinectWrapper.NuiSkeletonPositionIndex.HipCenter},
		{1, KinectWrapper.NuiSkeletonPositionIndex.Spine},
		{2, KinectWrapper.NuiSkeletonPositionIndex.ShoulderCenter},
		{3, KinectWrapper.NuiSkeletonPositionIndex.Head},
		
		{5, KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight},
		{6, KinectWrapper.NuiSkeletonPositionIndex.ElbowRight},
		{7, KinectWrapper.NuiSkeletonPositionIndex.WristRight},
		{8, KinectWrapper.NuiSkeletonPositionIndex.HandRight},
		
		{10, KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft},
		{11, KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft},
		{12, KinectWrapper.NuiSkeletonPositionIndex.WristLeft},
		{13, KinectWrapper.NuiSkeletonPositionIndex.HandLeft},
		
		{14, KinectWrapper.NuiSkeletonPositionIndex.HipRight},
		{15, KinectWrapper.NuiSkeletonPositionIndex.KneeRight},
		{16, KinectWrapper.NuiSkeletonPositionIndex.AnkleRight},
		{17, KinectWrapper.NuiSkeletonPositionIndex.FootRight},
		
		{18, KinectWrapper.NuiSkeletonPositionIndex.HipLeft},
		{19, KinectWrapper.NuiSkeletonPositionIndex.KneeLeft},
		{20, KinectWrapper.NuiSkeletonPositionIndex.AnkleLeft},
		{21, KinectWrapper.NuiSkeletonPositionIndex.FootLeft},
	};
	
	protected readonly Dictionary<int, List<KinectWrapper.NuiSkeletonPositionIndex>> specIndex2MirrorJointMap = new Dictionary<int, List<KinectWrapper.NuiSkeletonPositionIndex>>
	{
		{4, new List<KinectWrapper.NuiSkeletonPositionIndex> {KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight, KinectWrapper.NuiSkeletonPositionIndex.ShoulderCenter} },
		{9, new List<KinectWrapper.NuiSkeletonPositionIndex> {KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft, KinectWrapper.NuiSkeletonPositionIndex.ShoulderCenter} },
	};
	
}

