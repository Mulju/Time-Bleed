using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class TimeSphereSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject timeBind;
    private float spawnTimer;
    private float spawnPositionY;

    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = 0;
        spawnPositionY = 15;
    }

    // Update is called once per frame
    void Update()
    {
        if (!base.IsServer)
            return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer > 3)
        {
            TimeBindServer();
            spawnTimer = 0;
        }    
    }

    [ServerRpc(RequireOwnership = false)]
    public void TimeBindServer()
    {
        float x = Random.Range(-29, 29);
        float z = Random.Range(-58, 58);

        Vector3 randomPosition = new Vector3(x, spawnPositionY, z);

        TimeBind(randomPosition);
    }

    [ObserversRpc]
    public void TimeBind(Vector3 position)
    {
        GameObject timeBindInstance = Instantiate(timeBind, position, Quaternion.identity);
        Destroy(timeBindInstance, 25);
    }
}
