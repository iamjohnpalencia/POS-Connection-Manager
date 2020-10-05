Imports System.Windows.Forms

Public Class MainForm
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim C As Control
        For Each C In Me.Controls
            If TypeOf C Is MdiClient Then
                C.BackColor = Color.White
                Exit For
            End If
        Next
        C = Nothing
    End Sub
    Private Sub DatabaseConnectionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DatabaseConnectionToolStripMenuItem.Click
        Dim newMDIchild As New Connection()
        If Application.OpenForms().OfType(Of Connection).Any Then
        Else
            newMDIchild.MdiParent = Me
            newMDIchild.ShowIcon = False
            newMDIchild.Show()
        End If
    End Sub
End Class
