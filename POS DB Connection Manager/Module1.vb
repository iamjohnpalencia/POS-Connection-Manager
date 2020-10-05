Imports MySql.Data.MySqlClient
Module Module1
    Public Function ConvertB64ToString(str As String)
        Dim b As Byte() = Convert.FromBase64String(str)
        Dim byt2 = System.Text.Encoding.UTF8.GetString(b)
        Return byt2
    End Function
    Public Function ConvertToBase64(str As String)
        Dim byt As Byte() = System.Text.Encoding.UTF8.GetBytes(str)
        Dim byt2 = Convert.ToBase64String(byt)
        Return byt2
    End Function
    Public Function RemoveCharacter(ByVal stringToCleanUp, ByVal characterToRemove)
        ' replace the target with nothing
        ' Replace() returns a new String and does not modify the current one
        Return stringToCleanUp.Replace(characterToRemove, "")
    End Function
    Public Function TextboxIsEmpty(ByVal root As Control) As Boolean
        Dim ReturnThisThing As Boolean
        For Each tb As TextBox In root.Controls.OfType(Of TextBox)()
            If tb.Text = String.Empty Then
                ReturnThisThing = False
                Exit For
            Else
                ReturnThisThing = True
            End If
        Next
        Return ReturnThisThing
    End Function

    Public Sub TextBoxIsReadOnly(ByVal root As Control)
        For Each tb As TextBox In root.Controls.OfType(Of TextBox)()
            If tb.ReadOnly = True Then
                tb.ReadOnly = False
            Else
                tb.ReadOnly = True
            End If
        Next
    End Sub

End Module
