using UnityEditor;
using UnityEditor.Android;
using UnityEngine;
using System.IO;

class JetifierBuildProcessor : IPostGenerateGradleAndroidProject
{
    public int callbackOrder { get { return 999; } }
    public void OnPostGenerateGradleAndroidProject(string path)
    {
        string gradlePropertiesFile = path + "/gradle.properties";
        if (File.Exists(gradlePropertiesFile))
        {
            File.Delete(gradlePropertiesFile);
        }
        StreamWriter writer = File.CreateText(gradlePropertiesFile);
        writer.WriteLine("org.gradle.jvmargs=-Xmx4096M");
        writer.WriteLine("android.useAndroidX=true");
        writer.WriteLine("android.enableJetifier=true");
        //writer.WriteLine("android.jetifier.blacklist = com.android.support.support-.*\\.aar");
        writer.Flush();
        writer.Close();
    }
}