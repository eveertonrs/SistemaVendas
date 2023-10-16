Imports System.Data.SqlClient

Public Class Inicio

    Private Sub Inicio_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Listar()
    End Sub
    Private Sub SairToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SairToolStripMenuItem.Click
        Application.Exit()

    End Sub
    Private Sub FuncionariosToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FuncionariosToolStripMenuItem.Click
        Dim form = New funcionarios
        form.ShowDialog()
    End Sub
    Private Sub ClientesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ClientesToolStripMenuItem.Click
        Dim form = New Clientes
        form.ShowDialog()
    End Sub

    Private Sub RegistrarVendaToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RegistrarVendaToolStripMenuItem.Click
        Dim form = New RegistrarVendas
        form.ShowDialog()
    End Sub
    Private Sub Listar()
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter

        Try
            abrir()
            ' Use o SQL personalizado que seleciona os dados desejados
            da = New SqlDataAdapter("SELECT V.ClienteID, C.Nome AS NomeCliente, VI.Produto, VI.Quantidade, VI.PrecoUnitario, VI.TotalItem, V.DataVenda " & _
                                    "FROM tbdVenda V " & _
                                    "INNER JOIN tbdVendaItens VI ON V.VendaID = VI.VendaID " & _
                                    "INNER JOIN tbdCliente C ON V.ClienteID = C.ClienteID", conn)
            da.Fill(dt) ' Preencha o DataTable com os resultados

            ' Atribua o DataTable como a fonte de dados do DataGridView
            DataGridView1.DataSource = dt

            ' Oculte o cabeçalho das colunas
            DataGridView1.ColumnHeadersVisible = True
        Catch ex As Exception
            MessageBox.Show("Erro: " & ex.Message)
        Finally
            fechar()
        End Try
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Listar()
    End Sub
End Class