namespace EPiOptimiser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Reflection;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Configuration;
    using EPiServer.Framework.Configuration;
    using Microsoft.Build.Utilities;
    using System.Windows.Forms;

    class Program
    {
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("No project web.config path specified.");
                Console.ReadKey();
                return;
            }

            string sourcePath = args[0];

            if (!File.Exists(sourcePath))
            {
                Console.WriteLine("Could not open file \"" + sourcePath + "\".");
                try { Console.ReadKey(); }
                catch { }
                return;
            }

            var assemblyParser = new ParseAssembliesForPlugIns();

            StringBuilder sb = new StringBuilder();
            foreach (string assemblyName in assemblyParser.GetSafeToIgnoreAssemblies(sourcePath))
            {
                string remove = "<remove assembly=\"" + assemblyName + "\" />";
                Console.WriteLine(remove);
                sb.Append(remove + System.Environment.NewLine);
            }

            Clipboard.SetText(sb.ToString());

            Console.WriteLine();
            Console.WriteLine("========================================");
            Console.WriteLine("Remove assembly list copied to clipboard");
            Console.WriteLine("========================================");

            try { Console.ReadKey(); }
            catch { }
        }
    }
}




//namespace EPiPlugInScanner
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            var fileMap = new ConfigurationFileMap(@"C:\Projects\RICS\trunk\Rics.Web\web.config");
//            var configuration = ConfigurationManager.OpenMappedMachineConfiguration(fileMap);

//            //foreach (var sectionss in configuration.SectionGroups)
//            //{
//            //    Console.WriteLine(sectionss.ToString());
//            //}
//            ////var sectionGroup = configuration.GetSectionGroup(@"episerver.framework"); // This is the section group name, change to your needs
//            //var section = configuration.Sections.Get("scanAssembly"); // This is the section name, change to your needs

//            var section2 = (EPiServerFrameworkSection)configuration.Sections.Get("episerver.framework"); // This is the section name, change to your needs
//            foreach (var assemb in section2.ScanAssembly)
//            {
//                assemb.ToString();
//            }

//            //foreach (FileInfo info in new DirectoryInfo(".").GetFiles("*.dll"))
//            //{
//            //    try
//            //    {
//            //        bool isOKToRemove = false;

//            //        //Check if there are any MEF references
//            //        Assembly assembly = Assembly.LoadFile(info.FullName);
//            //        if (new AssemblyCatalog(assembly).Parts.Count<ComposablePartDefinition>() < 1)
//            //        {
//            //            isOKToRemove = true;
//            //        }

//            //        //Check if something has a plug in attribute before proceeding
//            //        Type[] types = assembly.GetTypes();
//            //        foreach (Type t in types)
//            //        {
//            //            foreach (object attrib in t.GetCustomAttributes(false))
//            //            {
//            //                //Console.WriteLine("==" + t.Name + "==");
//            //                //Console.WriteLine(t.Attributes.ToString());
//            //                if (attrib.GetType().IsSubclassOf(typeof(EPiServer.PlugIn.PlugInAttribute)) && !t.IsAbstract)
//            //                {
//            //                    isOKToRemove = false;
//            //                    break;
//            //                }
//            //            }
//            //            if (!isOKToRemove) break;
//            //        }

//            //        if (isOKToRemove)
//            //            Console.Out.WriteLine(string.Format("<remove assembly=\"{0}\" />", assembly.FullName.Split(new char[] { ',' })[0]));
//            //    }
//            //    catch (Exception exception)
//            //    {
//            //        //Console.WriteLine(info.FullName);
//            //        //Console.Error.WriteLine(string.Format("Error {0} in file {1}", exception.Message, info.Name));
//            //    }
//            //}
//            Console.ReadLine();
//        }
//    }
//}
