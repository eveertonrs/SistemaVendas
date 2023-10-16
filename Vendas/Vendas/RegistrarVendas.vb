Imports System.Data.SqlClient

Public Class RegistrarVendas

    Private vendaIDSelecionado As Integer ' Variável para armazenar o VendaID do registro selecionado

    Private Sub Produtos_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        DesabilitarCampos()
        limpar()
        btnSalvar.Enabled = False
        Listar()
        lbSaveEdit.Visible = False
        btnSaveEdit.Visible = False
        DataGridView1.ReadOnly = True
        cbCliente.ValueMember = "ClienteID"

        ' Exemplo de estilo para células
        DataGridView1.DefaultCellStyle.Font = New Font("Arial", 8)
        DataGridView1.DefaultCellStyle.ForeColor = Color.Black
        DataGridView1.DefaultCellStyle.BackColor = Color.White
        DataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray

        ' Exemplo de estilo para o cabeçalho das colunas
        DataGridView1.ColumnHeadersDefaultCellStyle.Font = New Font("Arial", 8, FontStyle.Bold)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkBlue

        ' Redimensionar colunas automaticamente
        DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells

        ' Personalizar a cor da seleção de linhas
        DataGridView1.DefaultCellStyle.SelectionBackColor = Color.DarkBlue
        DataGridView1.DefaultCellStyle.SelectionForeColor = Color.White

        ' Adicionar bordas ao DataGridView
        DataGridView1.BorderStyle = BorderStyle.Fixed3D

        Try
            abrir()
            Dim query As String = "SELECT ClienteID, Nome FROM tbdCliente"
            Dim command As New SqlCommand(query, conn)
            Dim adapter As New SqlDataAdapter(command)
            Dim dataTable As New DataTable()
            adapter.Fill(dataTable)

            cbCliente.DataSource = dataTable
            cbCliente.DisplayMember = "Nome"
            cbCliente.ValueMember = "ClienteID"
        Catch ex As Exception
            MessageBox.Show("Erro: " & ex.Message)
        Finally
            fechar()
        End Try
    End Sub
    Private Sub DesabilitarCampos()
        txtProduto.Enabled = False
        txtQtd.Enabled = False
        txtValorTotal.Enabled = False
        txtValorUnit.Enabled = False
        cbCliente.Enabled = False
        dtData.Enabled = False
    End Sub
    Private Sub HabilitarCampos()
        txtProduto.Enabled = True
        txtQtd.Enabled = True
        txtValorTotal.Enabled = True
        txtValorUnit.Enabled = True
        cbCliente.Enabled = True
        dtData.Enabled = True
    End Sub

    Private Sub limpar()
        txtProduto.Text = ""
        txtQtd.Text = ""
        txtValorTotal.Text = ""
        txtValorUnit.Text = ""
        cbCliente.Text = ""
        dtData.Text = ""
    End Sub
    Private Sub Listar()
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter

        Try
            abrir()
            ' Consulta SQL para unir as tabelas e obter todos os dados
            Dim query As String = "SELECT V.VendaID, C.Nome AS NomeCliente, V.DataVenda, VI.Produto, VI.Quantidade, VI.PrecoUnitario, VI.TotalItem " & _
                                 "FROM tbdVenda AS V " & _
                                 "INNER JOIN tbdCliente AS C ON V.ClienteID = C.ClienteID " & _
                                 "INNER JOIN tbdVendaItens AS VI ON V.VendaID = VI.VendaID"

            da = New SqlDataAdapter(query, conn)
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



    Private Sub btnNovo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNovo.Click
        HabilitarCampos()
        limpar()
        btnSalvar.Enabled = True
        lbSaveEdit.Visible = False
        btnSaveEdit.Visible = False
    End Sub

    Private Sub btnSalvar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSalvar.Click
        ' Coleta os dados do formulário
        Dim clienteID As Integer
        Dim dataVenda As DateTime = dtData.Value ' Use a data selecionada no DateTimePicker
        Dim produto As String = txtProduto.Text
        Dim qtd As Integer
        Dim valorunit As Decimal
        Dim valortotal As Decimal

        ' Verifica se os campos numéricos são válidos
        If Not Integer.TryParse(txtQtd.Text, qtd) OrElse qtd <= 0 OrElse
           Not Decimal.TryParse(txtValorUnit.Text, valorunit) OrElse valorunit <= 0 OrElse
           Not Decimal.TryParse(txtValorTotal.Text, valortotal) OrElse valortotal <= 0 Then
            MessageBox.Show("Os campos de quantidade, valor unitário e valor total devem conter valores numéricos válidos maiores que zero.")
            Return
        End If

        ' Validação dos campos obrigatórios
        If String.IsNullOrWhiteSpace(produto) OrElse cbCliente.SelectedValue Is Nothing Then
            MessageBox.Show("Preencha todos os campos obrigatórios corretamente.")
            Return
        End If

        ' Coleta o ClienteID selecionado no ComboBox
        clienteID = Integer.Parse(cbCliente.SelectedValue.ToString())

        ' Abre a conexão
        abrir()

        ' Variável para armazenar a mensagem de saída da inserção
        Dim mensagem As String = ""

        ' Variável para armazenar o VendaID gerado
        Dim vendaID As Integer = 0

        ' Inserindo a venda
        Dim insertVendaQuery As String = "INSERT INTO tbdVenda (ClienteID, DataVenda) VALUES (@ClienteID, @DataVenda); SELECT SCOPE_IDENTITY();"

        Using cmd As New SqlCommand(insertVendaQuery, conn)
            cmd.Parameters.Add(New SqlParameter("@ClienteID", clienteID))
            cmd.Parameters.Add(New SqlParameter("@DataVenda", dataVenda))
            vendaID = Convert.ToInt32(cmd.ExecuteScalar())
        End Using

        ' Inserindo o item de venda se o VendaID foi obtido com sucesso
        If vendaID > 0 Then
            Dim insertItemQuery As String = "INSERT INTO tbdVendaItens (VendaID, Produto, Quantidade, PrecoUnitario, TotalItem) VALUES (@VendaID, @Produto, @Quantidade, @PrecoUnitario, @TotalItem);"

            Using cmd As New SqlCommand(insertItemQuery, conn)
                cmd.Parameters.Add(New SqlParameter("@VendaID", vendaID))
                cmd.Parameters.Add(New SqlParameter("@Produto", produto))
                cmd.Parameters.Add(New SqlParameter("@Quantidade", qtd))
                cmd.Parameters.Add(New SqlParameter("@PrecoUnitario", valorunit))
                cmd.Parameters.Add(New SqlParameter("@TotalItem", valortotal))
                cmd.ExecuteNonQuery()
            End Using

            ' Exibe a mensagem para o usuário
            MessageBox.Show("Venda e item de venda inseridos com sucesso.")

            ' Fecha a conexão
            fechar()

            ' Limpa os campos após o salvamento
            limpar()

            ' Atualiza a lista de vendas ou faz qualquer outra ação necessária
            Listar()
        Else
            MessageBox.Show("Falha ao inserir a venda.")
        End If
    End Sub




    Private Sub DataGridView1_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub cbSexo_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbCliente.SelectedIndexChanged

    End Sub

    Private Sub btnExcluir_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExcluir.Click
        ' Verifique se um registro está selecionado
        If vendaIDSelecionado > 0 Then
            ' Abra a conexão
            abrir()

            ' Variável para armazenar a mensagem de saída da exclusão
            Dim mensagem As String = ""

            ' Excluir a venda e os itens de venda
            ExcluirVendaEItens(vendaIDSelecionado, mensagem)

            ' Exiba a mensagem resultante da exclusão
            MessageBox.Show(mensagem)

            ' Feche a conexão
            fechar()

            ' Atualize o DataGridView1 ou faça qualquer outra ação necessária
            Listar()
        Else
            MessageBox.Show("Selecione um registro para excluir.")
        End If
    End Sub

    Private Sub ExcluirVendaEItens(ByVal vendaID As Integer, ByRef mensagem As String)
        ' Exclua os itens de venda associados a esta venda
        Dim excluirItensQuery As String = "DELETE FROM tbdVendaItens WHERE VendaID = @VendaID;"

        Using cmd As New SqlCommand(excluirItensQuery, conn)
            cmd.Parameters.Add(New SqlParameter("@VendaID", vendaID))
            cmd.ExecuteNonQuery()
        End Using

        ' Exclua a venda
        Dim excluirVendaQuery As String = "DELETE FROM tbdVenda WHERE VendaID = @VendaID;"

        Using cmd As New SqlCommand(excluirVendaQuery, conn)
            cmd.Parameters.Add(New SqlParameter("@VendaID", vendaID))
            cmd.ExecuteNonQuery()
        End Using

        ' Defina a mensagem de saída
        mensagem = "Venda e itens de venda excluídos com sucesso."
    End Sub


    Private Sub btnSaveEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveEdit.Click
        ' Coleta os dados do formulário
        Dim produto As String = txtProduto.Text
        Dim quantidade As Integer = Integer.Parse(txtQtd.Text)
        Dim precoUnitario As Decimal = Decimal.Parse(txtValorUnit.Text)
        Dim totalItem As Decimal = Decimal.Parse(txtValorTotal.Text)

        abrir()

        ' Validação dos campos
        If String.IsNullOrWhiteSpace(produto) OrElse quantidade <= 0 OrElse precoUnitario <= 0 OrElse totalItem <= 0 Then
            MessageBox.Show("Preencha todos os campos obrigatórios corretamente.")
            Return
        End If

        ' Certifique-se de que o vendaIDSelecionado contenha o VendaItemID do item que está sendo editado.
        If vendaIDSelecionado > 0 Then
            ' Crie a consulta SQL de UPDATE
            Dim updateQuery As String = "UPDATE tbdVendaItens SET Produto = @Produto, Quantidade = @Quantidade, PrecoUnitario = @PrecoUnitario, TotalItem = @TotalItem WHERE VendaItemID = @VendaItemID"

            ' Execute a consulta SQL de UPDATE
            Using cmd As New SqlCommand(updateQuery, conn)
                cmd.Parameters.Add(New SqlParameter("@VendaItemID", vendaIDSelecionado))
                cmd.Parameters.Add(New SqlParameter("@Produto", produto))
                cmd.Parameters.Add(New SqlParameter("@Quantidade", quantidade))
                cmd.Parameters.Add(New SqlParameter("@PrecoUnitario", precoUnitario))
                cmd.Parameters.Add(New SqlParameter("@TotalItem", totalItem))

                Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                If rowsAffected > 0 Then
                    MessageBox.Show("Item de venda editado com sucesso.")
                Else
                    MessageBox.Show("Item de venda com o VendaItemID " & vendaIDSelecionado & " não encontrado.")
                End If
            End Using

            ' Fecha a conexão
            fechar()

            ' Limpa os campos após o salvamento
            limpar()

            ' Atualiza a lista de vendas ou faz qualquer outra ação necessária
            Listar()
        Else
            MessageBox.Show("Selecione um item de venda para editar.")
        End If
    End Sub

    Private Sub DataGridView1_CellClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        HabilitarCampos()
        If e.RowIndex >= 0 Then
            ' Obtém os valores das células da linha selecionada e preenche os campos do formulário
            dtData.Value = DateTime.Parse(DataGridView1.Rows(e.RowIndex).Cells("DataVenda").Value.ToString())
            txtProduto.Text = DataGridView1.Rows(e.RowIndex).Cells("Produto").Value.ToString()
            txtQtd.Text = DataGridView1.Rows(e.RowIndex).Cells("Quantidade").Value.ToString()
            txtValorUnit.Text = DataGridView1.Rows(e.RowIndex).Cells("PrecoUnitario").Value.ToString()
            txtValorTotal.Text = DataGridView1.Rows(e.RowIndex).Cells("TotalItem").Value.ToString()
            ' Atualize a variável vendaIDSelecionado
            vendaIDSelecionado = Integer.Parse(DataGridView1.Rows(e.RowIndex).Cells("VendaID").Value.ToString())
            ' Torna o botão btnSaveEdit visível
            btnSaveEdit.Visible = True
            lbSaveEdit.Visible = True
        End If
    End Sub



    Private Sub txtbuscar_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtbuscar.TextChanged
        ' Obtém o texto digitado no TextBox
        Dim filtro As String = txtbuscar.Text.Trim()

        ' Verifica se o filtro não está vazio
        If filtro <> "" Then
            ' Abre a conexão
            abrir()

            ' Consulta SQL para buscar os resultados que correspondem ao filtro
            Dim query As String = "SELECT V.VendaID, C.Nome AS NomeCliente, V.DataVenda, VI.Produto, VI.Quantidade, VI.PrecoUnitario, VI.TotalItem " & _
                                 "FROM tbdVenda AS V " & _
                                 "INNER JOIN tbdCliente AS C ON V.ClienteID = C.ClienteID " & _
                                 "INNER JOIN tbdVendaItens AS VI ON V.VendaID = VI.VendaID " & _
                                 "WHERE VI.Produto LIKE @filtro"

            ' Crie um adaptador de dados
            Dim adapter As New SqlDataAdapter(query, conn)

            ' Defina o parâmetro @filtro na consulta
            adapter.SelectCommand.Parameters.AddWithValue("@filtro", "%" & filtro & "%")

            ' Crie um DataTable para armazenar os resultados
            Dim dataTable As New DataTable()

            ' Preencha o DataTable com os resultados da consulta
            adapter.Fill(dataTable)

            ' Atribua o DataTable como a fonte de dados do DataGridView
            DataGridView1.DataSource = dataTable

            ' Feche a conexão
            fechar()
        Else
            ' Se o filtro estiver vazio, liste todos os resultados
            Listar()
        End If
    End Sub

End Class