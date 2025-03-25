using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MachineManager : MonoBehaviour
{
    // Global tick rate
    public float tickRate = 1.0f;
    private float tickTimer = 0f;
    
    // Reference to all machines in the scene
    private List<Machine> machines = new List<Machine>();
    
    // Singleton instance
    public static MachineManager Instance { get; private set; }
    
    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        // Find all machines in the scene at start
        FindAllMachines();
    }
    
    // Collect all machines in the scene
    public void FindAllMachines()
    {
        machines.Clear();
        machines.AddRange(FindObjectsByType<Machine>(FindObjectsSortMode.None));
    }
    
    // Add a new machine to the tracking list
    public void RegisterMachine(Machine machine)
    {
        if (!machines.Contains(machine))
            machines.Add(machine);
    }
    
    // Remove a machine from the tracking list
    public void UnregisterMachine(Machine machine)
    {
        if (machines.Contains(machine))
            machines.Remove(machine);
    }
    
    private void Update()
    {
        // Handle timing for ticks
        tickTimer += Time.deltaTime;
        
        if (tickTimer >= 1f / tickRate)
        {
            tickTimer = 0f;
            PerformTick();
        }
    }
    
    // Execute a simulation tick
    private void PerformTick()
    {
        // Shuffle machines to avoid bias in processing order
        Shuffle(machines);
        
        // Pre-tick phase
        foreach (Machine machine in machines)
        {
            machine.PreTick();
        }
        
        // Tick phase - may require multiple passes
        bool allProcessed = false;
        int maxPasses = machines.Count * machines.Count; // Worst case scenario
        int currentPass = 0;
        
        while (!allProcessed && currentPass < maxPasses)
        {
            allProcessed = true;
            
            foreach (Machine machine in machines)
            {
                bool processed = machine.Tick();
                if (!processed)
                    allProcessed = false;
            }
            
            currentPass++;
        }
        
        // Post-tick phase
        foreach (Machine machine in machines)
        {
            machine.PostTick();
        }
    }
    
    // Spawn an item on a machine (for testing)
    public void SpawnItemOnMachine(Machine machine)
    {
        if (machine != null && machine.outputStorage.Count < machine.maxOutputCapacity)
        {
            // Create a temporary item (it doesn't need a GameObject yet)
            Item item = new Item();
            machine.outputStorage.Add(item);
            machine.PostTick(); // Update visuals
        }
    }
    
    // Shuffle a list (Fisher-Yates algorithm)
    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        for (int i = 0; i < n; i++)
        {
            int r = i + Random.Range(0, n - i);
            T temp = list[i];
            list[i] = list[r];
            list[r] = temp;
        }
    }
}