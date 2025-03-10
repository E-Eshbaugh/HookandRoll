using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEditorAPI
{
    #region Request/Response Models
    
    public class CreateGameObjectRequest
    {
        public string name { get; set; }
        public string parentId { get; set; }
        public Vector3Data position { get; set; }
        public Vector3Data rotation { get; set; }
        public Vector3Data scale { get; set; }
    }
    
    public class UpdateGameObjectRequest
    {
        public string name { get; set; }
        public bool? active { get; set; }
        public string parentId { get; set; }
        public Vector3Data position { get; set; }
        public Vector3Data rotation { get; set; }
        public Vector3Data scale { get; set; }
    }
    
    public class AddComponentRequest
    {
        public string componentType { get; set; }
    }
    
    public class Vector3Data
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }
    
    #endregion
    
    public class EditorAPIServer : EditorWindow
    {
        private HttpListener listener;
        private Thread listenerThread;
        private bool isRunning = false;
        private string serverUrl = "http://localhost:8080/";
        private string apiKey = "";
        private bool requireApiKey = false;
        private List<string> logMessages = new List<string>();
        private Vector2 logScrollPosition;
        private Dictionary<string, GameObject> objectCache = new Dictionary<string, GameObject>();
        
        [MenuItem("Tools/Editor API Server")]
        public static void ShowWindow()
        {
            GetWindow<EditorAPIServer>("Editor API Server");
        }
        
        void OnEnable()
        {
            // Generate a random API key
            if (string.IsNullOrEmpty(apiKey))
            {
                apiKey = Guid.NewGuid().ToString("N");
            }
        }
        
        void OnDisable()
        {
            StopServer();
        }
        
        void OnGUI()
        {
            GUILayout.Label("Unity Editor API Server", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            serverUrl = EditorGUILayout.TextField("Server URL", serverUrl);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            requireApiKey = EditorGUILayout.Toggle("Require API Key", requireApiKey);
            EditorGUILayout.EndHorizontal();
            
            if (requireApiKey)
            {
                EditorGUILayout.BeginHorizontal();
                apiKey = EditorGUILayout.TextField("API Key", apiKey);
                if (GUILayout.Button("Generate", GUILayout.Width(100)))
                {
                    apiKey = Guid.NewGuid().ToString("N");
                }
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.Space();
            
            if (!isRunning)
            {
                if (GUILayout.Button("Start Server"))
                {
                    StartServer();
                }
            }
            else
            {
                if (GUILayout.Button("Stop Server"))
                {
                    StopServer();
                }
                
                EditorGUILayout.LabelField("Server Status: Running");
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Server Log");
            
            logScrollPosition = EditorGUILayout.BeginScrollView(logScrollPosition, GUILayout.Height(300));
            foreach (var logMessage in logMessages)
            {
                EditorGUILayout.LabelField(logMessage);
            }
            EditorGUILayout.EndScrollView();
            
            if (GUILayout.Button("Clear Log"))
            {
                logMessages.Clear();
            }
        }
        
        void AddLog(string message)
        {
            logMessages.Add($"[{DateTime.Now.ToString("HH:mm:ss")}] {message}");
            
            // Keep log at a reasonable size
            if (logMessages.Count > 100)
            {
                logMessages.RemoveAt(0);
            }
            
            // Force repaint to update the UI
            Repaint();
        }
        
        void StartServer()
        {
            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add(serverUrl);
                listener.Start();
                
                isRunning = true;
                
                listenerThread = new Thread(ListenForRequests);
                listenerThread.IsBackground = true;
                listenerThread.Start();
                
                AddLog($"Server started at {serverUrl}");
                
                if (requireApiKey)
                {
                    AddLog($"API Key: {apiKey}");
                }
            }
            catch (Exception ex)
            {
                AddLog($"Error starting server: {ex.Message}");
                isRunning = false;
            }
        }
        
        void StopServer()
        {
            if (listener != null)
            {
                listener.Stop();
                listener.Close();
                listener = null;
            }
            
            if (listenerThread != null && listenerThread.IsAlive)
            {
                listenerThread.Abort();
                listenerThread = null;
            }
            
            isRunning = false;
            AddLog("Server stopped");
        }
        
        void ListenForRequests()
        {
            while (listener != null && listener.IsListening)
            {
                try
                {
                    var context = listener.GetContext();
                    ThreadPool.QueueUserWorkItem(_ => ProcessRequest(context));
                }
                catch (Exception ex)
                {
                    if (listener != null && listener.IsListening)
                    {
                        AddLog($"Error while listening: {ex.Message}");
                    }
                }
            }
        }
        
        void ProcessRequest(HttpListenerContext context)
        {
            string endpoint = context.Request.Url.AbsolutePath;
            string method = context.Request.HttpMethod;
            
            try
            {
                // Validate API key if required
                if (requireApiKey)
                {
                    string providedApiKey = context.Request.Headers["X-API-Key"];
                    if (string.IsNullOrEmpty(providedApiKey) || providedApiKey != apiKey)
                    {
                        SendErrorResponse(context, HttpStatusCode.Unauthorized, "Invalid or missing API key");
                        return;
                    }
                }
                
                // Log the request
                AddLog($"{method} {endpoint}");
                
                // Ensure Unity operations are executed on the main thread
                EditorApplication.delayCall += () =>
                {
                    try
                    {
                        HandleEndpoint(context, endpoint, method);
                    }
                    catch (Exception ex)
                    {
                        SendErrorResponse(context, HttpStatusCode.InternalServerError, ex.Message);
                        AddLog($"Error handling request: {ex.Message}");
                    }
                };
            }
            catch (Exception ex)
            {
                SendErrorResponse(context, HttpStatusCode.InternalServerError, ex.Message);
                AddLog($"Error processing request: {ex.Message}");
            }
        }
        
        void HandleEndpoint(HttpListenerContext context, string endpoint, string method)
        {
            // API endpoints
            switch (endpoint)
            {
                // Project info
                case "/project":
                    if (method == "GET")
                    {
                        GetProjectInfo(context);
                    }
                    else
                    {
                        SendMethodNotAllowedResponse(context);
                    }
                    break;
                
                // Hierarchy endpoints
                case "/hierarchy":
                    if (method == "GET")
                    {
                        GetHierarchy(context);
                    }
                    else
                    {
                        SendMethodNotAllowedResponse(context);
                    }
                    break;
                
                // GameObject endpoints
                case "/gameobject/create":
                    if (method == "POST")
                    {
                        CreateGameObject(context);
                    }
                    else
                    {
                        SendMethodNotAllowedResponse(context);
                    }
                    break;
                
                // Components endpoints
                case "/components":
                    if (method == "GET")
                    {
                        GetAvailableComponents(context);
                    }
                    else
                    {
                        SendMethodNotAllowedResponse(context);
                    }
                    break;
                
                // Scenes endpoints
                case "/scenes":
                    if (method == "GET")
                    {
                        GetScenes(context);
                    }
                    else
                    {
                        SendMethodNotAllowedResponse(context);
                    }
                    break;
                
                // Inspector endpoints
                case "/inspector":
                    if (method == "GET")
                    {
                        GetInspectorData(context);
                    }
                    else
                    {
                        SendMethodNotAllowedResponse(context);
                    }
                    break;
                
                // Default
                default:
                    // Check if it's a pattern-based endpoint
                    if (endpoint.StartsWith("/gameobject/") && endpoint.Length > 12)
                    {
                        string objId = endpoint.Substring(12);
                        
                        // Check for specific object endpoints
                        if (endpoint.EndsWith("/components"))
                        {
                            objId = objId.Substring(0, objId.Length - 11); // Remove "/components"
                            if (method == "GET")
                            {
                                GetGameObjectComponents(context, objId);
                            }
                            else if (method == "POST")
                            {
                                AddComponentToGameObject(context, objId);
                            }
                            else
                            {
                                SendMethodNotAllowedResponse(context);
                            }
                        }
                        else if (endpoint.Contains("/component/"))
                        {
                            // Handle component operations
                            string[] parts = endpoint.Split(new[] { "/component/" }, StringSplitOptions.None);
                            if (parts.Length == 2)
                            {
                                objId = parts[0].Substring(12); // Remove "/gameobject/"
                                string componentId = parts[1];
                                
                                if (method == "GET")
                                {
                                    GetComponentProperties(context, objId, componentId);
                                }
                                else if (method == "PUT")
                                {
                                    UpdateComponentProperties(context, objId, componentId);
                                }
                                else if (method == "DELETE")
                                {
                                    RemoveComponent(context, objId, componentId);
                                }
                                else
                                {
                                    SendMethodNotAllowedResponse(context);
                                }
                            }
                            else
                            {
                                SendNotFoundResponse(context);
                            }
                        }
                        else
                        {
                            // Handle GameObject operations
                            if (method == "GET")
                            {
                                GetGameObject(context, objId);
                            }
                            else if (method == "PUT")
                            {
                                UpdateGameObject(context, objId);
                            }
                            else if (method == "DELETE")
                            {
                                DeleteGameObject(context, objId);
                            }
                            else
                            {
                                SendMethodNotAllowedResponse(context);
                            }
                        }
                    }
                    else if (endpoint == "/assets")
                    {
                        if (method == "GET")
                        {
                            GetAssets(context);
                        }
                        else
                        {
                            SendMethodNotAllowedResponse(context);
                        }
                    }
                    else
                    {
                        SendNotFoundResponse(context);
                    }
                    break;
            }
        }
        
        #region Endpoint Handlers
        
        void GetProjectInfo(HttpListenerContext context)
        {
            var projectInfo = new
            {
                name = PlayerSettings.productName,
                version = PlayerSettings.bundleVersion,
                path = Application.dataPath,
                unityVersion = Application.unityVersion
            };
            
            SendJsonResponse(context, projectInfo);
        }
        
        void GetHierarchy(HttpListenerContext context)
        {
            var scene = SceneManager.GetActiveScene();
            var rootObjects = scene.GetRootGameObjects();
            
            var hierarchy = new
            {
                sceneName = scene.name,
                sceneIndex = scene.buildIndex,
                isDirty = scene.isDirty,
                rootObjects = rootObjects.Select(go => SerializeGameObject(go, true)).ToArray()
            };
            
            SendJsonResponse(context, hierarchy);
        }
        
        void GetGameObject(HttpListenerContext context, string objectId)
        {
            GameObject go = FindGameObjectById(objectId);
            if (go == null)
            {
                SendNotFoundResponse(context, "GameObject not found");
                return;
            }
            
            var gameObjectData = SerializeGameObject(go, true);
            SendJsonResponse(context, gameObjectData);
        }
        
        void CreateGameObject(HttpListenerContext context)
        {
            string requestBody = new StreamReader(context.Request.InputStream).ReadToEnd();
            var requestData = JsonConvert.DeserializeObject<CreateGameObjectRequest>(requestBody);
            
            if (string.IsNullOrEmpty(requestData.name))
            {
                SendErrorResponse(context, HttpStatusCode.BadRequest, "GameObject name is required");
                return;
            }
            
            GameObject parent = null;
            if (!string.IsNullOrEmpty(requestData.parentId))
            {
                parent = FindGameObjectById(requestData.parentId);
                if (parent == null)
                {
                    SendErrorResponse(context, HttpStatusCode.BadRequest, "Parent GameObject not found");
                    return;
                }
            }
            
            GameObject newGo = new GameObject(requestData.name);
            if (parent != null)
            {
                newGo.transform.SetParent(parent.transform);
            }
            
            // Set transform properties if provided
            if (requestData.position != null)
            {
                newGo.transform.position = new Vector3(
                    requestData.position.x,
                    requestData.position.y,
                    requestData.position.z
                );
            }
            
            if (requestData.rotation != null)
            {
                newGo.transform.rotation = Quaternion.Euler(
                    requestData.rotation.x,
                    requestData.rotation.y,
                    requestData.rotation.z
                );
            }
            
            if (requestData.scale != null)
            {
                newGo.transform.localScale = new Vector3(
                    requestData.scale.x,
                    requestData.scale.y,
                    requestData.scale.z
                );
            }
            
            // Add to cache
            string id = newGo.GetInstanceID().ToString();
            objectCache[id] = newGo;
            
            var response = new
            {
                id = id,
                name = newGo.name,
                success = true
            };
            
            SendJsonResponse(context, response);
        }
        
        void UpdateGameObject(HttpListenerContext context, string objectId)
        {
            GameObject go = FindGameObjectById(objectId);
            if (go == null)
            {
                SendNotFoundResponse(context, "GameObject not found");
                return;
            }
            
            string requestBody = new StreamReader(context.Request.InputStream).ReadToEnd();
            var requestData = JsonConvert.DeserializeObject<UpdateGameObjectRequest>(requestBody);
            
            if (requestData.name != null)
            {
                go.name = requestData.name;
            }
            
            if (requestData.active.HasValue)
            {
                go.SetActive(requestData.active.Value);
            }
            
            if (requestData.position != null)
            {
                go.transform.position = new Vector3(
                    requestData.position.x,
                    requestData.position.y,
                    requestData.position.z
                );
            }
            
            if (requestData.rotation != null)
            {
                go.transform.rotation = Quaternion.Euler(
                    requestData.rotation.x,
                    requestData.rotation.y,
                    requestData.rotation.z
                );
            }
            
            if (requestData.scale != null)
            {
                go.transform.localScale = new Vector3(
                    requestData.scale.x,
                    requestData.scale.y,
                    requestData.scale.z
                );
            }
            
            if (requestData.parentId != null)
            {
                if (string.IsNullOrEmpty(requestData.parentId))
                {
                    // Set to scene root
                    go.transform.SetParent(null);
                }
                else
                {
                    // Find parent
                    GameObject parent = FindGameObjectById(requestData.parentId);
                    if (parent != null)
                    {
                        go.transform.SetParent(parent.transform);
                    }
                    else
                    {
                        SendErrorResponse(context, HttpStatusCode.BadRequest, "Specified parent GameObject not found");
                        return;
                    }
                }
            }
            
            var response = new
            {
                id = objectId,
                name = go.name,
                success = true
            };
            
            SendJsonResponse(context, response);
        }
        
        void DeleteGameObject(HttpListenerContext context, string objectId)
        {
            GameObject go = FindGameObjectById(objectId);
            if (go == null)
            {
                SendNotFoundResponse(context, "GameObject not found");
                return;
            }
            
            // Remove from cache
            objectCache.Remove(objectId);
            
            // Destroy the GameObject
            DestroyImmediate(go);
            
            var response = new
            {
                id = objectId,
                success = true
            };
            
            SendJsonResponse(context, response);
        }
        
        void GetGameObjectComponents(HttpListenerContext context, string objectId)
        {
            GameObject go = FindGameObjectById(objectId);
            if (go == null)
            {
                SendNotFoundResponse(context, "GameObject not found");
                return;
            }
            
            Component[] components = go.GetComponents<Component>();
            var componentData = components.Select(c => SerializeComponent(c)).ToArray();
            
            SendJsonResponse(context, componentData);
        }
        
        void AddComponentToGameObject(HttpListenerContext context, string objectId)
        {
            GameObject go = FindGameObjectById(objectId);
            if (go == null)
            {
                SendNotFoundResponse(context, "GameObject not found");
                return;
            }
            
            string requestBody = new StreamReader(context.Request.InputStream).ReadToEnd();
            var requestData = JsonConvert.DeserializeObject<AddComponentRequest>(requestBody);
            
            if (string.IsNullOrEmpty(requestData.componentType))
            {
                SendErrorResponse(context, HttpStatusCode.BadRequest, "Component type is required");
                return;
            }
            
            // Find the component type
            Type componentType = FindComponentType(requestData.componentType);
            if (componentType == null)
            {
                SendErrorResponse(context, HttpStatusCode.BadRequest, $"Component type '{requestData.componentType}' not found");
                return;
            }
            
            // Check if component can be added
            if (!CanAddComponent(go, componentType))
            {
                SendErrorResponse(context, HttpStatusCode.BadRequest, $"Cannot add component '{requestData.componentType}' to this GameObject");
                return;
            }
            
            // Add the component
            Component component = go.AddComponent(componentType);
            
            // Return the component data
            var componentData = SerializeComponent(component);
            SendJsonResponse(context, componentData);
        }
        
        void GetComponentProperties(HttpListenerContext context, string objectId, string componentId)
        {
            GameObject go = FindGameObjectById(objectId);
            if (go == null)
            {
                SendNotFoundResponse(context, "GameObject not found");
                return;
            }
            
            Component component = FindComponentById(go, componentId);
            if (component == null)
            {
                SendNotFoundResponse(context, "Component not found");
                return;
            }
            
            var componentData = SerializeComponent(component, true);
            SendJsonResponse(context, componentData);
        }
        
        void UpdateComponentProperties(HttpListenerContext context, string objectId, string componentId)
        {
            GameObject go = FindGameObjectById(objectId);
            if (go == null)
            {
                SendNotFoundResponse(context, "GameObject not found");
                return;
            }
            
            Component component = FindComponentById(go, componentId);
            if (component == null)
            {
                SendNotFoundResponse(context, "Component not found");
                return;
            }
            
            string requestBody = new StreamReader(context.Request.InputStream).ReadToEnd();
            var requestData = JsonConvert.DeserializeObject<Dictionary<string, object>>(requestBody);
            
            // Update the properties
            bool success = UpdateComponentProperties(component, requestData);
            
            if (!success)
            {
                SendErrorResponse(context, HttpStatusCode.BadRequest, "Failed to update component properties");
                return;
            }
            
            var componentData = SerializeComponent(component, true);
            SendJsonResponse(context, componentData);
        }
        
        void RemoveComponent(HttpListenerContext context, string objectId, string componentId)
        {
            GameObject go = FindGameObjectById(objectId);
            if (go == null)
            {
                SendNotFoundResponse(context, "GameObject not found");
                return;
            }
            
            Component component = FindComponentById(go, componentId);
            if (component == null)
            {
                SendNotFoundResponse(context, "Component not found");
                return;
            }
            
            // Cannot remove Transform component
            if (component is Transform)
            {
                SendErrorResponse(context, HttpStatusCode.BadRequest, "Cannot remove Transform component");
                return;
            }
            
            // Destroy the component
            DestroyImmediate(component);
            
            var response = new
            {
                success = true
            };
            
            SendJsonResponse(context, response);
        }
        
        void GetAvailableComponents(HttpListenerContext context)
        {
            var componentTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(Component).IsAssignableFrom(type) && !type.IsAbstract)
                .Select(type => new
                {
                    name = type.Name,
                    fullName = type.FullName,
                    assembly = type.Assembly.GetName().Name
                })
                .OrderBy(c => c.name)
                .ToArray();
            
            SendJsonResponse(context, componentTypes);
        }
        
        void GetScenes(HttpListenerContext context)
        {
            int sceneCount = SceneManager.sceneCount;
            var scenes = new List<object>();
            
            for (int i = 0; i < sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                scenes.Add(new
                {
                    name = scene.name,
                    path = scene.path,
                    buildIndex = scene.buildIndex,
                    isLoaded = scene.isLoaded,
                    isActive = scene == SceneManager.GetActiveScene()
                });
            }
            
            // Add build scenes
            var buildScenes = new List<object>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var buildScene = EditorBuildSettings.scenes[i];
                buildScenes.Add(new
                {
                    path = buildScene.path,
                    enabled = buildScene.enabled
                });
            }
            
            var result = new
            {
                loadedScenes = scenes.ToArray(),
                buildScenes = buildScenes.ToArray()
            };
            
            SendJsonResponse(context, result);
        }
        
        void GetInspectorData(HttpListenerContext context)
        {
            string query = context.Request.Url.Query;
            string objectId = null;
            
            if (!string.IsNullOrEmpty(query) && query.StartsWith("?id="))
            {
                objectId = query.Substring(4);
            }
            
            if (string.IsNullOrEmpty(objectId))
            {
                SendErrorResponse(context, HttpStatusCode.BadRequest, "Object ID is required");
                return;
            }
            
            GameObject go = FindGameObjectById(objectId);
            if (go == null)
            {
                SendNotFoundResponse(context, "GameObject not found");
                return;
            }
            
            var inspectorData = new
            {
                gameObject = SerializeGameObject(go, false),
                components = go.GetComponents<Component>().Select(c => SerializeComponent(c, true)).ToArray()
            };
            
            SendJsonResponse(context, inspectorData);
        }
        
        void GetAssets(HttpListenerContext context)
        {
            var assets = new List<object>();
            string[] guids = AssetDatabase.FindAssets("t:Object");
            
            foreach (string guid in guids.Take(100)) // Limit to 100 assets to avoid overwhelming response
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                try
                {
                    var asset = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
                    if (asset != null)
                    {
                        assets.Add(new
                        {
                            name = asset.name,
                            path = path,
                            type = asset.GetType().Name
                        });
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error loading asset at {path}: {ex.Message}");
                }
            }
            
            var result = new
            {
                count = assets.Count,
                assets = assets.ToArray()
            };
            
            SendJsonResponse(context, result);
        }
        
        #endregion
        
        #region Helper Methods
        
        GameObject FindGameObjectById(string id)
        {
            // First check the cache
            if (objectCache.TryGetValue(id, out GameObject cachedGo))
            {
                if (cachedGo != null) // Ensure it's not destroyed
                {
                    return cachedGo;
                }
                else
                {
                    // Remove from cache if destroyed
                    objectCache.Remove(id);
                }
            }
            
            // Try to parse as an instance ID
            if (int.TryParse(id, out int instanceId))
            {
                GameObject go = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
                if (go != null)
                {
                    // Add to cache
                    objectCache[id] = go;
                    return go;
                }
            }
            
            // Search by path if it looks like a path
            if (id.Contains("/"))
            {
                GameObject go = GameObject.Find(id);
                if (go != null)
                {
                    // Add to cache
                    objectCache[go.GetInstanceID().ToString()] = go;
                    return go;
                }
            }
            
            return null;
        }
        
        Component FindComponentById(GameObject gameObject, string componentId)
        {
            if (gameObject == null)
            {
                return null;
            }
            
            // Try to parse as an index
            if (int.TryParse(componentId, out int index))
            {
                var components = gameObject.GetComponents<Component>();
                if (index >= 0 && index < components.Length)
                {
                    return components[index];
                }
            }
            
            // Try to parse as an instance ID
            if (int.TryParse(componentId, out int instanceId))
            {
                Component component = EditorUtility.InstanceIDToObject(instanceId) as Component;
                if (component != null && component.gameObject == gameObject)
                {
                    return component;
                }
            }
            
            // Try to find by type name
            var foundComponents = gameObject.GetComponents<Component>()
                .Where(c => c.GetType().Name == componentId || c.GetType().FullName == componentId)
                .ToArray();
            
            return foundComponents.FirstOrDefault();
        }
        
        Type FindComponentType(string typeName)
        {
            // Search for the type in all loaded assemblies
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(typeName);
                if (type != null && typeof(Component).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    return type;
                }
            }
            
            // Try just the type name without namespace
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = assembly.GetTypes()
                    .Where(t => t.Name == typeName && typeof(Component).IsAssignableFrom(t) && !t.IsAbstract)
                    .ToArray();
                
                if (types.Length > 0)
                {
                    return types[0];
                }
            }
            
            return null;
        }
        
        object ConvertValue(object value, Type targetType)
        {
            if (value == null)
            {
                return null;
            }
            
            // Handle Vector3, Quaternion, etc.
            if (targetType == typeof(Vector3) && value is Newtonsoft.Json.Linq.JObject jObj)
            {
                try
                {
                    float x = jObj["x"]?.ToObject<float>() ?? 0f;
                    float y = jObj["y"]?.ToObject<float>() ?? 0f;
                    float z = jObj["z"]?.ToObject<float>() ?? 0f;
                    return new Vector3(x, y, z);
                }
                catch
                {
                    return Vector3.zero;
                }
            }
            else if (targetType == typeof(Vector2) && value is Newtonsoft.Json.Linq.JObject jObjV2)
            {
                try
                {
                    float x = jObjV2["x"]?.ToObject<float>() ?? 0f;
                    float y = jObjV2["y"]?.ToObject<float>() ?? 0f;
                    return new Vector2(x, y);
                }
                catch
                {
                    return Vector2.zero;
                }
            }
            else if (targetType == typeof(Quaternion) && value is Newtonsoft.Json.Linq.JObject jObjQ)
            {
                try
                {
                    // Check if it's Euler angles representation
                    if (jObjQ.ContainsKey("eulerAngles") || (jObjQ.ContainsKey("x") && !jObjQ.ContainsKey("w")))
                    {
                        float x = jObjQ["x"]?.ToObject<float>() ?? 0f;
                        float y = jObjQ["y"]?.ToObject<float>() ?? 0f;
                        float z = jObjQ["z"]?.ToObject<float>() ?? 0f;
                        return Quaternion.Euler(x, y, z);
                    }
                    else
                    {
                        float x = jObjQ["x"]?.ToObject<float>() ?? 0f;
                        float y = jObjQ["y"]?.ToObject<float>() ?? 0f;
                        float z = jObjQ["z"]?.ToObject<float>() ?? 0f;
                        float w = jObjQ["w"]?.ToObject<float>() ?? 1f;
                        return new Quaternion(x, y, z, w);
                    }
                }
                catch
                {
                    return Quaternion.identity;
                }
            }
            else if (targetType == typeof(Color) && value is Newtonsoft.Json.Linq.JObject jObjC)
            {
                try
                {
                    float r = jObjC["r"]?.ToObject<float>() ?? 0f;
                    float g = jObjC["g"]?.ToObject<float>() ?? 0f;
                    float b = jObjC["b"]?.ToObject<float>() ?? 0f;
                    float a = jObjC["a"]?.ToObject<float>() ?? 1f;
                    return new Color(r, g, b, a);
                }
                catch
                {
                    return Color.white;
                }
            }
            
            // Handle Enum types
            if (targetType.IsEnum)
            {
                try
                {
                    if (value is string enumString)
                    {
                        return Enum.Parse(targetType, enumString);
                    }
                    else if (value is int enumInt)
                    {
                        return Enum.ToObject(targetType, enumInt);
                    }
                }
                catch
                {
                    return Enum.GetValues(targetType).GetValue(0); // Return first enum value as default
                }
            }
            
            // Handle arrays and lists
            if (targetType.IsArray && value is Newtonsoft.Json.Linq.JArray jArray)
            {
                Type elementType = targetType.GetElementType();
                Array array = Array.CreateInstance(elementType, jArray.Count);
                
                for (int i = 0; i < jArray.Count; i++)
                {
                    array.SetValue(ConvertValue(jArray[i], elementType), i);
                }
                
                return array;
            }
            
            // Handle GameObject references (by ID)
            if (targetType == typeof(GameObject) && value is string goId)
            {
                return FindGameObjectById(goId);
            }
            
            // Handle basic type conversion
            try
            {
                if (targetType == typeof(int) && value is long longVal)
                {
                    return (int)longVal;
                }
                else if (targetType == typeof(float) && (value is double doubleVal))
                {
                    return (float)doubleVal;
                }
                else if (targetType == typeof(bool) && value != null)
                {
                    return Convert.ToBoolean(value);
                }
                else if (value is string stringVal && targetType != typeof(string))
                {
                    // Try to convert string to target type
                    return Convert.ChangeType(stringVal, targetType);
                }
                else if (targetType.IsAssignableFrom(value?.GetType()))
                {
                    return value;
                }
                else
                {
                    return Convert.ChangeType(value, targetType);
                }
            }
            catch
            {
                // Return default value for the type
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            }
        }
        
        bool CanAddComponent(GameObject gameObject, Type componentType)
        {
            // Check if GameObject already has the component (for unique components)
            var existingComponents = gameObject.GetComponents(componentType);
            if (existingComponents.Length > 0)
            {
                // Check if component allows multiple instances
                var attrs = componentType.GetCustomAttributes(typeof(DisallowMultipleComponent), true);
                if (attrs.Length > 0)
                {
                    return false;
                }
            }
            
            // TODO: Add more checks as needed
            return true;
        }
        
        bool UpdateComponentProperties(Component component, Dictionary<string, object> properties)
        {
            if (component == null || properties == null)
            {
                return false;
            }
            
            try
            {
                Type componentType = component.GetType();
                
                foreach (var property in properties)
                {
                    // Get the property or field info
                    PropertyInfo propInfo = componentType.GetProperty(property.Key);
                    FieldInfo fieldInfo = propInfo == null ? componentType.GetField(property.Key) : null;
                    
                    if (propInfo != null && propInfo.CanWrite)
                    {
                        // Convert value to the property type
                        object convertedValue = ConvertValue(property.Value, propInfo.PropertyType);
                        propInfo.SetValue(component, convertedValue);
                    }
                    else if (fieldInfo != null)
                    {
                        // Convert value to the field type
                        object convertedValue = ConvertValue(property.Value, fieldInfo.FieldType);
                        fieldInfo.SetValue(component, convertedValue);
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error updating component properties: {ex.Message}");
                return false;
            }
        }
    }
}