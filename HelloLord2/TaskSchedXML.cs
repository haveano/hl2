using System.IO;

public partial class Klasy
{
    public static void TaskSchedXML(string interval, string exeFile, string workDir)
    {
        string text = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>"
                + "<Task version=\"1.2\" xmlns=\"http://schemas.microsoft.com/windows/2004/02/mit/task\">"
                +  "<RegistrationInfo>"
                //+    "<Date>2018-03-05T16:55:46</Date>"
                //+    "<Author>LORD2</Author>"
                +    "<URI>\\Lord2</URI>"
                +  "</RegistrationInfo>"
                +  "<Triggers>"
                +    "<TimeTrigger>"
                +      "<Repetition>"
                //+        "<Interval>PT1M</Interval>"    //interwal 1 minuta
                +        "<Interval>PT"+interval+"M</Interval>" //interwal 15 minut
                +        "<StopAtDurationEnd>false</StopAtDurationEnd>"
                +      "</Repetition>"
                +      "<StartBoundary>2018-03-05T16:55:00</StartBoundary>"
                +      "<Enabled>true</Enabled>"
                +    "</TimeTrigger>"
                +  "</Triggers>"
                +  "<Principals>"
                +    "<Principal id=\"Author\">"
                +      "<LogonType>InteractiveToken</LogonType>"
                +      "<RunLevel>LeastPrivilege</RunLevel>"
                +    "</Principal>"
                +  "</Principals>"
                +  "<Settings>"
                +    "<MultipleInstancesPolicy>IgnoreNew</MultipleInstancesPolicy>"
                +    "<DisallowStartIfOnBatteries>false</DisallowStartIfOnBatteries>"
                +    "<StopIfGoingOnBatteries>true</StopIfGoingOnBatteries>"
                +    "<AllowHardTerminate>true</AllowHardTerminate>"
                +    "<StartWhenAvailable>false</StartWhenAvailable>"
                +    "<RunOnlyIfNetworkAvailable>false</RunOnlyIfNetworkAvailable>"
                +    "<IdleSettings>"
                +      "<StopOnIdleEnd>true</StopOnIdleEnd>"
                +      "<RestartOnIdle>false</RestartOnIdle>"
                +    "</IdleSettings>"
                +    "<AllowStartOnDemand>true</AllowStartOnDemand>"
                +    "<Enabled>true</Enabled>"
                +    "<Hidden>false</Hidden>"
                +    "<RunOnlyIfIdle>false</RunOnlyIfIdle>"
                +    "<WakeToRun>false</WakeToRun>"
                +    "<ExecutionTimeLimit>PT72H</ExecutionTimeLimit>"
                +    "<Priority>7</Priority>"
                +  "</Settings>"
                +  "<Actions Context=\"Author\">"
                +    "<Exec>"
                //+      "<Command>%localappdata%\\HelloLord2\\HelloLord2.exe</Command>"
                //+      "<WorkingDirectory>%localappdata%\\HelloLord2\\</WorkingDirectory>"
                +      "<Command>"+exeFile+"</Command>"
                +      "<WorkingDirectory>"+workDir+"</WorkingDirectory>"
                +    "</Exec>"
                +  "</Actions>"
                +"</Task>";

        string temp = Path.GetTempPath();
        File.WriteAllText(@temp+"hl2.xml", text);
    }
}

    /*



    */
