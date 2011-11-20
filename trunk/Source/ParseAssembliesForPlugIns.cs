namespace EPiOptimiser
{
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Parse assemblies in a folder that implement any kind of EPiServer plug in mechanism
    /// </summary>
    public class ParseAssembliesForPlugIns
    {
        private string _targetPath;

        /// <summary>
        /// Parse a target folder for assemblies that implement any kind of EPiServer plug in 
        /// mechanism and return a list of assemblies that are safe to ignore
        /// </summary>
        /// <param name="targetPath">The path to scan, e.g. C:\projects\MyProject\wwwroot\bin</param>
        /// <returns>A list of assemblies that should be safe to ignore</returns>
        public IList<string> GetSafeToIgnoreAssemblies(string targetPath)
        {
            List<string> safeRemoveAssemblies = new List<string>();

            //Ensure we always parse out the target path
            this._targetPath = targetPath.Substring(0, targetPath.Length - Path.GetFileName(targetPath).Length);
            this._targetPath += @"\bin";

            //Add a custom assembly resolver to ensure we are always probing in the target path
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            foreach (FileInfo info in new DirectoryInfo(this._targetPath).GetFiles("*.dll"))
            {
                try
                {

                    //Check if there are any MEF references, to check for the EPiServer initialisation system
                    Assembly assembly = Assembly.LoadFile(info.FullName);
                    if (new AssemblyCatalog(assembly).Parts.Count<ComposablePartDefinition>() > 0)
                    {
                        continue;
                    }

                    bool isOKToRemove = true;

                    //Check if something has a plug in attribute before proceeding
                    Type[] types = assembly.GetTypes();
                    foreach (Type t in types)
                    {
                        foreach (object attrib in t.GetCustomAttributes(false))
                        {
                            //if (attrib.GetType().IsSubclassOf(typeof(EPiServer.PlugIn.PlugInAttribute)) && !t.IsAbstract)
                            if (attrib.GetType().Name.Contains("PlugInAttribute") && !t.IsAbstract)
                            {
                                isOKToRemove = false;
                                break;
                            }
                        }
                        if (!isOKToRemove)
                            break;
                    }

                    if (isOKToRemove)
                        safeRemoveAssemblies.Add(assembly.FullName.Split(new char[] { ',' })[0]);
                }
                catch
                {
                    //Really don't like doing this, but the assembly couldn't be loaded so we can't be sure therefore it must be ignored
                }
            }

            return safeRemoveAssemblies;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assemblyPath = this._targetPath + @"\" + args.Name.Split(new char[] { ',' })[0] + @".dll";
            return Assembly.LoadFile(assemblyPath);
        }
    }
}
