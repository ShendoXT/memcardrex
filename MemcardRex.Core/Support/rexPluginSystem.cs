using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace MemcardRex
{
    //Metadata containing descriptive plugin properties
    public struct pluginMetadata
    {
        public string pluginName;
        public string pluginAuthor;
        public string pluginSupportedGames;
    }

    public class rexPluginSystem
    {
        List<Assembly> loadedAssemblies = new List<Assembly>();
        public List<pluginMetadata> assembliesMetadata = new List<pluginMetadata>();

        //Load all available plugins
        public void fetchPlugins(string pluginDirectory)
        {
            //Prepare a clean data
            Assembly currentAssembly = null;
            List<string> assemblyTypes = new List<string>();
            pluginMetadata currentMetadata = new pluginMetadata();
            loadedAssemblies = new List<Assembly>();
            assembliesMetadata = new List<pluginMetadata>();

            //Check if the plugins directory exist
            if (Directory.Exists(pluginDirectory))
            {
                //Get all dll files from the plugins directory
                string[] filesInDirectory = Directory.GetFiles(pluginDirectory, "*.dll");

                //Load assemblies and check if rexPluginInterface is implemented
                foreach(string dirFile in filesInDirectory)
                {
                    currentAssembly = null;
                    assemblyTypes = new List<string>();
                    currentMetadata = new pluginMetadata();

                    try
                    {
                        //Load assembly
                        currentAssembly = Assembly.LoadFile(dirFile);

                        //Load assembly types
                        foreach(Type loadedTypes in currentAssembly.GetTypes())
                        {
                            assemblyTypes.Add(loadedTypes.ToString());
                        }

                        //Check if interface is properly implemented
                        if(assemblyTypes.Contains("rexPluginSystem.rexPluginInterfaceV2") && assemblyTypes.Contains("rexPluginSystem.rexPlugin"))
                        {
                            //Add validated MemcardRex plugin to the trusted assemblies list
                            loadedAssemblies.Add(currentAssembly);

                            //Load plugin metadata
                            currentMetadata.pluginName = getPluginName(loadedAssemblies.Count - 1);
                            currentMetadata.pluginAuthor = getPluginAuthor(loadedAssemblies.Count - 1);
                            currentMetadata.pluginSupportedGames = getPluginSupportedGames(loadedAssemblies.Count - 1);

                            //Update global metadata
                            assembliesMetadata.Add(currentMetadata);
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.Write(ex.Message);
                    }
                }
            }
        }

        //Return plugins which support editing of the given product code
        public int[] getSupportedPlugins(string productCode)
        {
            //Check if there are any loaded assemblies
            if (loadedAssemblies.Count > 0)
            {
                List<int> assembliesIndex = new List<int>();
                List<string> assemblyProdCode = new List<string>();

                //Cycle through each loaded assembly
                for (int i = 0; i < loadedAssemblies.Count; i++)
                {
                    //Clean previous data
                    assemblyProdCode.Clear();

                    //Get supported product codes
                    assemblyProdCode.AddRange(getSupportedProductCodes(i));

                    //Check if the product code is supported
                    if (assemblyProdCode.Contains(productCode) || assemblyProdCode.Contains("*.*"))
                    {
                        //Add assembly to the list
                        assembliesIndex.Add(i);
                    }
                }

                //Return processed data
                if (assembliesIndex.Count > 0)
                {
                    return assembliesIndex.ToArray();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        //Execute a specified method contained in the assembly
        string executeMethodString(int assemblyIndex, string methodName)
        {
            //Check if assembly list contains any members
            if (loadedAssemblies.Count > 0)
            {
                Type assemblyType = loadedAssemblies[assemblyIndex].GetType("rexPluginSystem.rexPlugin");
                var tempObject = Activator.CreateInstance(assemblyType);

                return (string)assemblyType.InvokeMember(methodName, BindingFlags.Default | BindingFlags.InvokeMethod, null, tempObject, null);
            }
            else
            {
                return null;
            }
        }

        //Execute a specified method contained in the assembly
        string[] executeMethodArray(int assemblyIndex, string methodName)
        {
            //Check if assembly list contains any members
            if (loadedAssemblies.Count > 0)
            {
                Type assemblyType = loadedAssemblies[assemblyIndex].GetType("rexPluginSystem.rexPlugin");
                var tempObject = Activator.CreateInstance(assemblyType);

                return (string[])assemblyType.InvokeMember(methodName, BindingFlags.Default | BindingFlags.InvokeMethod, null, tempObject, null);
            }
            else
            {
                return null;
            }
        }

        //Execute a specified method contained in the assembly
        byte[] executeMethodByte(int assemblyIndex, string methodName, byte[] gameSaveData, string saveProductCode)
        {
            //Check if assembly list contains any members
            if (loadedAssemblies.Count > 0)
            {
                Type assemblyType = loadedAssemblies[assemblyIndex].GetType("rexPluginSystem.rexPlugin");
                var tempObject = Activator.CreateInstance(assemblyType);

                return (byte[])assemblyType.InvokeMember(methodName, BindingFlags.Default | BindingFlags.InvokeMethod, null, tempObject, new object[] { gameSaveData, saveProductCode });
            }
            else
            {
                return null;
            }
        }

        //Execute a specified method contained in the assembly
        void executeMethodVoid(int assemblyIndex, string methodName, IntPtr? parentHandle)
        {
            //Check if assembly list contains any members
            if (loadedAssemblies.Count > 0)
            {
                Type assemblyType = loadedAssemblies[assemblyIndex].GetType("rexPluginSystem.rexPlugin");
                var tempObject = Activator.CreateInstance(assemblyType);

                object[] outObj = (parentHandle != IntPtr.Zero) 
                ? new object[] { parentHandle } 
                : null;

                assemblyType.InvokeMember(methodName, BindingFlags.Default | BindingFlags.InvokeMethod, null, tempObject, outObj);
            }
        }

        //
        //Pass through implementation of the methods contained by the plugins
        //

        //Get Name of the selected plugin
        string getPluginName(int assemblyIndex)
        {
            return executeMethodString(assemblyIndex, "getPluginName");
        }

        //Get Author of the selected plugin
        string getPluginAuthor(int assemblyIndex)
        {
            return executeMethodString(assemblyIndex, "getPluginAuthor");
        }

        //Get supported games of the selected plugin
        string getPluginSupportedGames(int assemblyIndex)
        {
            return executeMethodString(assemblyIndex, "getPluginSupportedGames");
        }

        //Get supported product codes of the selected plugin
        string[] getSupportedProductCodes(int assemblyIndex)
        {
            return executeMethodArray(assemblyIndex, "getSupportedProductCodes");
        }

        //Send host window id
        public void setWindowParent(int assemblyIndex, IntPtr parentHandle)
        {
            executeMethodVoid(assemblyIndex, "setWindowParent", parentHandle);
        }

        //Call plugin and pass data to it
        public byte[] editSaveData(int assemblyIndex, byte[] gameSaveData, string saveProductCode)
        {
            return executeMethodByte(assemblyIndex, "editSaveData", gameSaveData, saveProductCode);
        }

        //Show plugin about dialog
        public void showAboutDialog(int assemblyIndex)
        {
            executeMethodVoid(assemblyIndex, "showAboutDialog", IntPtr.Zero);
        }

        //Show plugin config dialog
        public void showConfigDialog(int assemblyIndex)
        {
            executeMethodVoid(assemblyIndex, "showConfigDialog", IntPtr.Zero);
        }
    }
}
