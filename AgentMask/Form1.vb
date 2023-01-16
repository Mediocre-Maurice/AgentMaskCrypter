Imports System.Text
Imports System.Threading

Public Class Form1

    Public Shared validchars As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"
    Public Shared rand As New Random()

    Public Resources_Parent = Randomi(rand.Next(3, 16))

    Public Shared Iconpath As String
    Public Shared assmb As String

    Public Shared trans As Boolean

    Public Shared Function Randomi(ByVal lenght As Integer) As String
        Dim Chr As String = "asdfghjklqwertyuiopmnbvcxz"
        Dim sb As New System.Text.StringBuilder()
        For i As Integer = 1 To lenght
            Dim idx As Integer = rand.Next(0, Chr.Length)
            sb.Append(Chr.Substring(idx, 1))
        Next
        Return sb.ToString
    End Function

    Public Sub New()
        InitializeComponent()
        Me.Opacity = 0
        Methods.FadeInMain(Me, 20)
    End Sub
    Private Sub Guna2CheckBox1_CheckedChanged(sender As Object, e As EventArgs)
        If Guna2CheckBox1.CheckedState = True Then
            Dim OfD As New OpenFileDialog
            With OfD
                .Title = "Select File"
                .Filter = "(*.exe|*.exe"
                If OfD.ShowDialog() = DialogResult.OK Then
                    assmb = OfD.FileName

                Else
                    assmb = Nothing
                    Guna2CheckBox1.CheckedState = False
                End If

            End With


        End If
        If Guna2CheckBox1.CheckedState = False Then
            assmb = Nothing
            Guna2CheckBox1.CheckedState = False
        End If
    End Sub

    Private Sub Guna2CheckBox2_CheckedChanged(sender As Object, e As EventArgs)
        If Guna2CheckBox2.CheckedState = True Then
            Dim OfD As New OpenFileDialog
            With OfD
                .Title = "Select Icon"
                .Filter = "(*.exe;*.ico;)|*.exe;*.ico"
                .InitialDirectory = AppDomain.CurrentDomain.BaseDirectory & "Icons"
                If OfD.ShowDialog() = DialogResult.OK Then
                    If OfD.FileName.ToLower.EndsWith(".exe") Then
                        Iconpath = Methods.GetIcon(OfD.FileName)
                        PictureBox1.ImageLocation = (Iconpath)
                    Else
                        Iconpath = OfD.FileName
                        PictureBox1.ImageLocation = (Iconpath)
                    End If
                Else
                    Iconpath = Nothing
                    PictureBox1.Image = Nothing
                    Guna2CheckBox2.CheckedState = False
                End If

            End With


        End If
        If Guna2CheckBox2.CheckedState = False Then
            Iconpath = Nothing
            PictureBox1.Image = Nothing
            Guna2CheckBox2.CheckedState = False
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try

            Guna2ComboBox1.SelectedIndex = 0
            Guna2ComboBox2.SelectedIndex = 0
            Methods.Load()
            Methods.ReferenceQuik()
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Sub

    Private Sub Form1_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        Try
            If trans Then Me.Opacity = 1.0
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Sub

    Private Sub Form1_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate
        Try
            Me.Opacity = 0.95
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs)
        Dim OfD As New OpenFileDialog
        With OfD
            .Title = "Select Assembly"
            .Filter = "(*.exe|*.exe"
            If OfD.ShowDialog() = DialogResult.OK Then
                If Methods.CheckNet(OfD.FileName) Then
                    Guna2TextBox1.Text = OfD.FileName
                Else
                    MessageBox.Show(OfD.FileName, "Is Not A .NET Assembly")
                End If
            Else
                Guna2TextBox1.Text = "..."
            End If

        End With
    End Sub

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        If Not Guna2TextBox1.Text = "..." Then
            Try

                Dim saveFileDialog As SaveFileDialog = New SaveFileDialog With {
                    .Filter = "(*.exe)|*.exe",
                    .OverwritePrompt = False,
                    .FileName = "Encrypted"
                }

                If saveFileDialog.ShowDialog() = DialogResult.OK Then

                    Guna2Button2.Text = "Wait..."
                    Guna2Button2.Enabled = False

                    Dim sb As New StringBuilder()
                    Dim rand As New Random()
                    For i As Integer = 1 To 17
                        Dim idx As Integer = rand.Next(0, validchars.Length)
                        Dim randomChar As Char = validchars(idx)
                        sb.Append(randomChar)
                    Next i
                    Dim randomString = sb.ToString()
                    Codedom.Compiler(saveFileDialog.FileName, Resources_Parent, randomString)
                End If
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        Else
            Try
                For i = 0 To 2
                    Me.Left = Me.DesktopLocation.X + 30
                    Thread.Sleep(40)
                    Me.Left = Me.DesktopLocation.X - 30
                    Thread.Sleep(40)
                Next
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
            End Try
        End If
    End Sub


    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Try
            Application.Exit()
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Sub

    Private Sub Guna2TextBox1_TextChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub Guna2TextBox1_DragEnter(sender As Object, e As DragEventArgs)
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub Guna2TextBox1_DragDrop(sender As Object, e As DragEventArgs)
        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)
        For Each path In files
            If IO.File.Exists(path) Then
                If IO.Path.GetExtension(path).ToLower = ".exe" Then
                    If Methods.CheckNet(path) Then
                        Guna2TextBox1.Text = path
                    Else
                        Guna2TextBox1.Text = "..."
                        MessageBox.Show(path, "Is Not A .NET Assembly")
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub Guna2Button1_Click_1(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        Dim OfD As New OpenFileDialog
        With OfD
            .Title = "Select Assembly"
            .Filter = "(*.exe|*.exe"
            If OfD.ShowDialog() = DialogResult.OK Then

                If Methods.CheckNet(OfD.FileName) Then
                    Guna2TextBox1.Text = OfD.FileName
                Else
                    MessageBox.Show(OfD.FileName, "Is Not A .NET Assembly")
                End If
            Else
                Guna2TextBox1.Text = "..."
            End If

        End With
    End Sub

    Private Sub Guna2CheckBox1_Click(sender As Object, e As EventArgs) Handles Guna2CheckBox1.Click
        If Guna2CheckBox1.CheckedState = True Then
            Dim OfD As New OpenFileDialog
            With OfD
                .Title = "Select File"
                .Filter = "(*.exe|*.exe"
                If OfD.ShowDialog() = DialogResult.OK Then
                    assmb = OfD.FileName

                Else
                    assmb = Nothing
                    Guna2CheckBox1.CheckedState = False
                End If

            End With


        End If
        If Guna2CheckBox1.CheckedState = False Then
            assmb = Nothing
            Guna2CheckBox1.CheckedState = False
        End If
    End Sub

    Private Sub Guna2CheckBox2_Click(sender As Object, e As EventArgs) Handles Guna2CheckBox2.Click
        If Guna2CheckBox2.CheckedState = True Then
            Dim OfD As New OpenFileDialog
            With OfD
                .Title = "Select Icon"
                .Filter = "(*.exe;*.ico;)|*.exe;*.ico"
                .InitialDirectory = AppDomain.CurrentDomain.BaseDirectory & "Icons"
                If OfD.ShowDialog() = DialogResult.OK Then
                    If OfD.FileName.ToLower.EndsWith(".exe") Then
                        Iconpath = Methods.GetIcon(OfD.FileName)
                        PictureBox1.ImageLocation = (Iconpath)
                    Else
                        Iconpath = OfD.FileName
                        PictureBox1.ImageLocation = (Iconpath)
                    End If
                Else
                    Iconpath = Nothing
                    PictureBox1.Image = Nothing
                    Guna2CheckBox2.CheckedState = False
                End If

            End With


        End If
        If Guna2CheckBox2.CheckedState = False Then
            Iconpath = Nothing
            PictureBox1.Image = Nothing
            Guna2CheckBox2.CheckedState = False
        End If
    End Sub
End Class
