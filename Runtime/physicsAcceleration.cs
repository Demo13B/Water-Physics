using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using System;
using UnityEngine.UIElements;

public class physicsAcceleration : MonoBehaviour
{
    public NNModel modelSource;
    private Model runtimeModel;
    private IWorker mWorker;
    private Rigidbody particleRigidbody;
    private MeshRenderer materialAttributes;
    private Vector3 position;
    private Vector3 velocity;
    private Material material;
    private float waitTime = 1.0f;
    private float timer = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        particleRigidbody = GetComponent<Rigidbody>();
        materialAttributes = GetComponent<MeshRenderer>();
        runtimeModel = ModelLoader.Load(modelSource);
        mWorker = WorkerFactory.CreateWorker(WorkerFactory.Type.Compute, runtimeModel);
    }

    // Update is called once per frame
    void Update()
    {
        // Time counter
        timer += Time.deltaTime;

        // This code is executed every second
        if (timer >= waitTime) {
            // Obtaining data
            position = transform.position;
            velocity = particleRigidbody.velocity;
            material = materialAttributes.material;
            float[] dataArray = {position[0], position[1], position[2], velocity[0], velocity[1], velocity[2]};
            int[] shape = {1, 6};

            // Running model
            Tensor input = new Tensor(shape, dataArray, "Position and velocity data");
            mWorker.Execute(input);
            Tensor output = mWorker.PeekOutput();
            input.Dispose();

            // Changing position
            transform.position = new Vector3(output[0], output[1], output[2]);
            output.Dispose();

            timer = 0.0f;
        }
    }
}
