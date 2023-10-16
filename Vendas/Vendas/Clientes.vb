Imports System.Data.SqlClient

Public Class Clientes
    Private Sub Clientes_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        DesabilitarCampos()
        limpar()
        btnSalvar.Enabled = False
        Listar()
        btnSaveEdit.Visible = False
        lbSaveEdit.Visible = False
        DataGridView1.ReadOnly = True
        DataGridView1.Columns("ClienteID").Visible = False

        ' Exemplo de estilo para células
        DataGridView1.DefaultCellStyle.Font = New Font("Arial", 10)
        DataGridView1.DefaultCellStyle.ForeColor = Color.Black
        DataGridView1.DefaultCellStyle.BackColor = Color.White
        DataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray

        ' Exemplo de estilo para o cabeçalho das colunas
        DataGridView1.ColumnHeadersDefaultCellStyle.Font = New Font("Arial", 10, FontStyle.Bold)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkBlue

        ' Redimensionar colunas automaticamente
        DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells

        ' Personalizar a cor da seleção de linhas
        DataGridView1.DefaultCellStyle.SelectionBackColor = Color.DarkBlue
        DataGridView1.DefaultCellStyle.SelectionForeColor = Color.White

        ' Adicionar bordas ao DataGridView
        DataGridView1.BorderStyle = BorderStyle.Fixed3D
    End Sub
    Private Sub DesabilitarCampos()
        txtNome.Enabled = False
        txtEndereco.Enabled = False
        txtCidade.Enabled = False
        txtUF.Enabled = False
    End Sub
    Private Sub HabilitarCampos()
        txtNome.Enabled = True
        txtEndereco.Enabled = True
        txtCidade.Enabled = True
        txtUF.Enabled = True
    End Sub
    Private Sub limpar()
        txtNome.Text = ""
        txtEndereco.Text = ""
        txtCidade.Text = ""
        txtUF.Text = ""
    End Sub

    Private Sub Listar()
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter

        Try
            abrir()
            da = New SqlDataAdapter("SELECT * FROM tbdCliente", conn)
            da.Fill(dt) ' Preencha o DataTable com os resultados

            ' Atribua o DataTable como a fonte de dados do DataGridView
            DataGridView1.DataSource = dt

            ' Oculte o cabeçalho das colunas
            DataGridView1.ColumnHeadersVisible = False
        Catch ex As Exception
            MessageBox.Show("Erro: " & ex.Message)
        Finally
            fechar()
        End Try
    End Sub

    Private Sub btnNovo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        ' Limpa os campos e prepara para adicionar um novo cliente
        txtNome.Text = ""
        txtEndereco.Text = ""
        txtCidade.Text = ""
        txtUF.Text = ""
    End Sub

    Private Sub DataGridView1_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        ' Verifique se a célula clicada não é o cabeçalho da coluna
        If e.RowIndex >= 0 Then
            ' Obtenha o índice da linha selecionada
            Dim rowIndex As Integer = e.RowIndex

            ' Obtenha os valores da linha selecionada
            Dim nome As String = DataGridView1.Rows(rowIndex).Cells("Nome").Value.ToString()
            Dim endereco As String = DataGridView1.Rows(rowIndex).Cells("Endereco").Value.ToString()
            Dim cidade As String = DataGridView1.Rows(rowIndex).Cells("Cidade").Value.ToString()
            Dim uf As String = DataGridView1.Rows(rowIndex).Cells("UF").Value.ToString()

            ' Preencha os campos do formulário com os valores obtidos
            txtNome.Text = nome
            txtEndereco.Text = endereco
            txtCidade.Text = cidade
            txtUF.Text = uf

            ' Habilite os campos para edição
            HabilitarCampos()
            lbSaveEdit.Visible = True
            btnSaveEdit.Visible = True
            btnSalvar.Enabled = False
        End If
    End Sub

    Private Sub btnNovo_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNovo.Click
        HabilitarCampos()
        limpar()
        btnSalvar.Enabled = True
        btnSaveEdit.Visible = False
        lbSaveEdit.Visible = False
    End Sub
    Private Sub btnSaveEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveEdit.Click
        ' Coleta os dados do formulário
        Dim nome As String = txtNome.Text
        Dim endereco As String = txtEndereco.Text
        Dim celular As String = txtCidade.Text
        Dim email As String = txtUF.Text

        ' Validação dos campos
        If String.IsNullOrWhiteSpace(nome) Then
            MessageBox.Show("Preencha todos os campos obrigatórios (Nome, Cidade).")
            Return
        End If

        ' Abre a conexão
        abrir()

        ' Chama a stored procedure sp_editarfunc
        Using cmd As New SqlCommand("sp_editarCliente", conn)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.Add(New SqlParameter("@Nome", nome))
            cmd.Parameters.Add(New SqlParameter("@Endereco", endereco))
            cmd.Parameters.Add(New SqlParameter("@Cidade", celular))
            cmd.Parameters.Add(New SqlParameter("@UF", email))
            cmd.Parameters.Add(New SqlParameter("@mensagem", SqlDbType.VarChar, 100))
            cmd.Parameters("@mensagem").Direction = ParameterDirection.Output

            cmd.ExecuteNonQuery()

            ' Captura a mensagem de saída
            Dim mensagem As String = cmd.Parameters("@mensagem").Value.ToString()

            ' Exibe a mensagem para o usuário
            MessageBox.Show(mensagem)
        End Using

        ' Fecha a conexão
        fechar()

        ' Limpa os campos após o salvamento
        limpar()

        ' Atualiza a lista de funcionários
        Listar()
    End Sub

    Private Sub btnSalvar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSalvar.Click
        ' Coleta os dados do formulário
        Dim nome As String = txtNome.Text
        Dim endereco As String = txtEndereco.Text
        Dim cidade As String = txtCidade.Text
        Dim uf As String = txtUF.Text

        ' Validação dos campos
        If String.IsNullOrWhiteSpace(nome) Then
            MessageBox.Show("Preencha o campo Nome.")
            Return
        End If

        ' Abre a conexão
        abrir()

        ' Chama a stored procedure sp_salvarCliente
        Using cmd As New SqlCommand("sp_salvarCliente", conn)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.Add(New SqlParameter("@Nome", nome))
            cmd.Parameters.Add(New SqlParameter("@Endereco", endereco))
            cmd.Parameters.Add(New SqlParameter("@Cidade", cidade))
            cmd.Parameters.Add(New SqlParameter("@UF", uf))
            cmd.Parameters.Add(New SqlParameter("@mensagem", SqlDbType.VarChar, 100))
            cmd.Parameters("@mensagem").Direction = ParameterDirection.Output

            cmd.ExecuteNonQuery()

            ' Captura a mensagem de saída
            Dim mensagem As String = cmd.Parameters("@mensagem").Value.ToString()

            ' Exibe a mensagem para o usuário
            MessageBox.Show(mensagem)
        End Using

        ' Fecha a conexão
        fechar()

        ' Limpa os campos após o salvamento
        limpar()

        ' Atualiza a lista de funcionários
        Listar()
    End Sub

    Private Sub btnExcluir_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExcluir.Click
        ' Coleta o nome do cliente a ser excluído
        Dim nome As String = txtNome.Text

        ' Validação do campo Nome
        If String.IsNullOrWhiteSpace(nome) Then
            MessageBox.Show("Informe o Nome do cliente que deseja excluir.")
            Return
        End If

        ' Abre a conexão
        abrir()

        ' Chama a stored procedure sp_excluirCliente
        Using cmd As New SqlCommand("sp_excluirCliente", conn)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.Add(New SqlParameter("@Nome", nome))
            cmd.Parameters.Add(New SqlParameter("@mensagem", SqlDbType.VarChar, 100))
            cmd.Parameters("@mensagem").Direction = ParameterDirection.Output

            cmd.ExecuteNonQuery()

            ' Captura a mensagem de saída
            Dim mensagem As String = cmd.Parameters("@mensagem").Value.ToString()

            ' Exibe a mensagem para o usuário
            MessageBox.Show(mensagem)
        End Using

        ' Fecha a conexão
        fechar()

        ' Limpa o campo após a exclusão
        txtNome.Text = ""
        Listar()
    End Sub

    Private Sub Label1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label1.Click

    End Sub
    Private Sub PesquisarPorNome(ByVal nome As String)
        ' Abra a conexão
        abrir()

        ' Chame a stored procedure sp_buscarfuncnome
        Using cmd As New SqlCommand("sp_buscarClienteNome", conn)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.Add(New SqlParameter("@nome", nome))
            cmd.Parameters.Add(New SqlParameter("@mensagem", SqlDbType.VarChar, 100))
            cmd.Parameters("@mensagem").Direction = ParameterDirection.Output

            Dim dt As New DataTable
            Dim da As New SqlDataAdapter(cmd)

            Try
                cmd.ExecuteNonQuery()
                da.Fill(dt)
                DataGridView1.DataSource = dt

                Dim mensagem As String = cmd.Parameters("@mensagem").Value.ToString()
                If Not String.IsNullOrWhiteSpace(mensagem) Then
                    MessageBox.Show(mensagem)
                End If
            Catch ex As Exception
                MessageBox.Show("Erro: " & ex.Message)
            End Try
        End Using

        ' Feche a conexão
        fechar()
    End Sub

    Private Sub txtbuscar_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtbuscar.TextChanged
        Dim nome As String = txtbuscar.Text
        PesquisarPorNome(nome)
    End Sub
End Class
