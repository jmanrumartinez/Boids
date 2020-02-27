using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour {
    [SerializeField] GameObject agentPrefab;

    [SerializeField] int numBoids = 10;

    Agent[] agents;

    [SerializeField] float agentRadius = 5.0f;

    [SerializeField] float separationWeight = 1.0f, cohesionWeight = 1.0f, alignmentWeight = 1.0f;

    private void Awake() {
        List<Agent> agentlist = new List<Agent>();

        for (int i = 0; i < numBoids; i++) {
            Vector3 position = Vector3.up * Random.Range(0, 10)
                + Vector3.right * Random.Range(0, 10) + Vector3.forward * Random.Range(0, 10);

            Agent newAgent = Instantiate(agentPrefab, position, Quaternion.identity).GetComponent<Agent>();
            newAgent.SetRadius(agentRadius); 
            agentlist.Add(newAgent);

        }
        agents = agentlist.ToArray();
    }

    // Update is called once per frame
    void Update() {
        foreach (Agent a in agents) {
            a.velocity = a.vel;
            a.neightbours.Clear();
            a.checkNeightbours();
            calculateCohesion(a);
            calculateSeparation(a);
            calculateAlignment(a);
            a.updateAgent();
        }
    }

    void calculateSeparation(Agent a) {
        Vector3 separationVector = Vector3.zero;
        foreach (Agent neightbour in a.neightbours) {
            float distance = Vector3.Distance(a.transform.position, neightbour.transform.position);
            distance /= a.radius;
            distance = 1 - distance;
            separationVector += distance * (a.transform.position - neightbour.transform.position).normalized * separationWeight;


        }
        a.addForce(separationVector, Agent.DEBUGforceType.SEPARATION);
    }

    void calculateCohesion(Agent a) {
        Vector3 centralPosition = new Vector3();

        foreach (Agent neightbour in a.neightbours) {
            centralPosition += neightbour.transform.position; 
        }
        centralPosition += a.transform.position;
        centralPosition /= a.neightbours.Count + 1;
        a.addForce((centralPosition - a.transform.position) * cohesionWeight, Agent.DEBUGforceType.COHESION);
    }

    void calculateAlignment(Agent a) {
        Vector3 dirVec = new Vector3();

        foreach (Agent neightbour in a.neightbours) {
            dirVec += neightbour.velocity; 
        }

        dirVec += a.velocity;
        dirVec /= a.neightbours.Count + 1;
        a.addForce(dirVec, Agent.DEBUGforceType.ALIGNMENT);
    }
}
