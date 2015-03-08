using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// todo: later on, we might want to consider only recording certain maximum length of frames, not the whole history
// todo: state of wheel colliders might be problematic..
public class PositionRecorder : MonoBehaviour {
    public int framesPerRewind = 3;

    private class Record
    {
        private Vector3 pos;
        private Quaternion rotation;
        private Vector3 velocity;
        private Vector3 angularVelocity;


        public Record(GameObject go)
        {
            pos = go.transform.position;
            rotation = go.transform.rotation;
            velocity = go.rigidbody.velocity;
            angularVelocity = go.rigidbody.angularVelocity;
        }

        public void Apply(GameObject go) {
            go.transform.position = pos;
            go.transform.rotation = rotation;
            go.rigidbody.velocity = velocity;
            go.rigidbody.angularVelocity = angularVelocity;
        }
    }

    private Stack<Record> history;

	// Use this for initialization
	void Start () {
        history = new Stack<Record>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Input.GetButton("Rewind Time"))
        {
            for (int i = 0; i < framesPerRewind && history.Count > 0; i++)
            {
                history.Pop();
            }
            if (history.Count <= 0)
            {
                return;
            }
            Record r = history.Pop();
            r.Apply(gameObject);
        }
        else
        {
            // continue recording
            history.Push(new Record(gameObject));
        }
	}
}
