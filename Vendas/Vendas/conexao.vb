Imports System.Data.SqlClient

Module conexao
    Public conn As New SqlConnection("Server=DESKTOP-V2L11PA; Database=****; User ID=master;Password=****")

    Sub abrir()
        If conn.State = 0 Then
            conn.Open()
        End If
    End Sub

    Sub fechar()
        If conn.State = 1 Then
            conn.Close()
        End If
    End Sub
End Module
