Imports System.Data.SqlClient

Public Class funcionarios

    Private Sub TextBox2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtNome.TextChanged

    End Sub
    Private Sub Label2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label2.Click

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNovo.Click
        HabilitarCampos()
        limpar()
        btnSalvar.Enabled = True
        lbSaveEdit.Visible = False
        btnSaveEdit.Visible = False
    End Sub

    Private Sub funcionarios_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        DesabilitarCampos()
        limpar()
        btnSalvar.Enabled = False
        Listar()
        lbSaveEdit.Visible = False
        btnSaveEdit.Visible = False
        DataGridView1.ReadOnly = True
        DataGridView1.Columns("id_func").Visible = False


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
    End Sub
    Private Sub DesabilitarCampos()
        txtNome.Enabled = False
        txtCelular.Enabled = False
        txtCPF.Enabled = False
        txtEmail.Enabled = False
        txtEndereco.Enabled = False
        cbSexo.Enabled = False
        dtData.Enabled = False
    End Sub
    Private Sub HabilitarCampos()
        txtNome.Enabled = True
        txtCelular.Enabled = True
        txtCPF.Enabled = True
        txtEmail.Enabled = True
        txtEndereco.Enabled = True
        cbSexo.Enabled = True
        dtData.Enabled = True
    End Sub

    Private Sub limpar()
        txtNome.Text = ""
        txtCelular.Text = ""
        txtCPF.Text = ""
        txtEmail.Text = ""
        txtEndereco.Text = ""
        cbSexo.Text = ""
        dtData.Text = ""
        dtData.Text = Now
    End Sub

    Private Sub Listar()
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter

        Try
            abrir()
            da = New SqlDataAdapter("SELECT * FROM tbtfuncionarios", conn)
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



    Private Sub DataGridView1_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub
    Private Sub DataGridView1_CellClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        ' Verifique se a célula clicada não é o cabeçalho da coluna
        If e.RowIndex >= 0 Then
            ' Obtenha o índice da linha selecionada
            Dim rowIndex As Integer = e.RowIndex

            ' Obtenha os valores da linha selecionada
            Dim nome As String = DataGridView1.Rows(rowIndex).Cells("nome").Value.ToString()
            Dim cpf As String = DataGridView1.Rows(rowIndex).Cells("cpf").Value.ToString()
            Dim sexo As String = DataGridView1.Rows(rowIndex).Cells("sexo").Value.ToString()
            Dim endereco As String = DataGridView1.Rows(rowIndex).Cells("endereco").Value.ToString()
            Dim celular As String = DataGridView1.Rows(rowIndex).Cells("celular").Value.ToString()
            Dim email As String = DataGridView1.Rows(rowIndex).Cells("email").Value.ToString()

            ' Preencha os campos do formulário com os valores obtidos
            txtNome.Text = nome
            txtCPF.Text = cpf
            cbSexo.Text = sexo
            txtEndereco.Text = endereco
            txtCelular.Text = celular
            txtEmail.Text = email

            ' Habilite os campos para edição
            HabilitarCampos()
            lbSaveEdit.Visible = True
            btnSaveEdit.Visible = True
            btnSalvar.Enabled = False
        End If
    End Sub


    Private Sub btnSalvar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSalvar.Click
        ' Coleta os dados do formulário
        Dim nome As String = txtNome.Text
        Dim sexo As String = cbSexo.Text
        Dim cpf As String = txtCPF.Text
        Dim endereco As String = txtEndereco.Text
        Dim celular As String = txtCelular.Text
        Dim email As String = txtEmail.Text
        Dim dataContrato As DateTime = dtData.Value

        ' Validação dos campos
        If String.IsNullOrWhiteSpace(nome) OrElse String.IsNullOrWhiteSpace(sexo) OrElse String.IsNullOrWhiteSpace(cpf) Then
            MessageBox.Show("Preencha todos os campos obrigatórios (Nome, Sexo, CPF).")
            Return
        End If

        ' Abre a conexão
        abrir()

        ' Chama a stored procedure sp_salvarfunc
        Using cmd As New SqlCommand("sp_salvarfunc", conn)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.Add(New SqlParameter("@nome", nome))
            cmd.Parameters.Add(New SqlParameter("@sexo", sexo))
            cmd.Parameters.Add(New SqlParameter("@cpf", cpf))
            cmd.Parameters.Add(New SqlParameter("@endereco", endereco))
            cmd.Parameters.Add(New SqlParameter("@celular", celular))
            cmd.Parameters.Add(New SqlParameter("@email", email))
            cmd.Parameters.Add(New SqlParameter("@data_contrato", dataContrato))
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

    Private Sub Label13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label13.Click

    End Sub



    Private Sub dtData_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtData.ValueChanged

    End Sub

    Private Sub btnSaveEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveEdit.Click
        ' Coleta os dados do formulário
        Dim nome As String = txtNome.Text
        Dim sexo As String = cbSexo.Text
        Dim cpf As String = txtCPF.Text
        Dim endereco As String = txtEndereco.Text
        Dim celular As String = txtCelular.Text
        Dim email As String = txtEmail.Text
        Dim dataContrato As DateTime = dtData.Value

        ' Validação dos campos
        If String.IsNullOrWhiteSpace(nome) OrElse String.IsNullOrWhiteSpace(sexo) OrElse String.IsNullOrWhiteSpace(cpf) Then
            MessageBox.Show("Preencha todos os campos obrigatórios (Nome, Sexo, CPF).")
            Return
        End If

        ' Abre a conexão
        abrir()

        ' Chama a stored procedure sp_editarfunc
        Using cmd As New SqlCommand("sp_editarfunc", conn)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.Add(New SqlParameter("@nome", nome))
            cmd.Parameters.Add(New SqlParameter("@sexo", sexo))
            cmd.Parameters.Add(New SqlParameter("@cpf", cpf))
            cmd.Parameters.Add(New SqlParameter("@endereco", endereco))
            cmd.Parameters.Add(New SqlParameter("@celular", celular))
            cmd.Parameters.Add(New SqlParameter("@email", email))
            cmd.Parameters.Add(New SqlParameter("@data_contrato", dataContrato))
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
        ' Coleta o CPF do funcionário a ser excluído
        Dim cpf As String = txtCPF.Text

        ' Validação do campo CPF
        If String.IsNullOrWhiteSpace(cpf) Then
            MessageBox.Show("Informe o CPF do funcionário que deseja excluir.")
            Return
        End If

        ' Abre a conexão
        abrir()

        ' Chama a stored procedure sp_excluirfunc
        Using cmd As New SqlCommand("sp_excluirfunc", conn)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.Add(New SqlParameter("@cpf", cpf))
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
        txtCPF.Text = ""

        ' Atualiza a lista de funcionários
        Listar()
    End Sub

    Private Sub Label12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub PesquisarPorNome(ByVal nome As String)
        ' Abra a conexão
        abrir()

        ' Chame a stored procedure sp_buscarfuncnome
        Using cmd As New SqlCommand("sp_buscarfuncnome", conn)
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