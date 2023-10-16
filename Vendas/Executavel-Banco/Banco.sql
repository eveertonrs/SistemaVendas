CREATE TABLE tbtfuncionarios(
	id_func INT PRIMARY KEY not null IDENTITY,
	nome VARCHAR(60) not null,
	sexo VARCHAR(20) not null,
	cpf VARCHAR(30) not null,
	endereco VARCHAR (100),
	celular VARCHAR (30),
	email VARCHAR (30),
	data_contrato DATETIME
);
-- Criação da tabela tbdCliente
CREATE TABLE tbdCliente (
    ClienteID INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    Nome VARCHAR(255),
    Endereco VARCHAR(255),
    Cidade VARCHAR(100),
    UF CHAR(2)
);
CREATE TABLE tbdVenda (
    VendaID INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    ClienteID INT NOT NULL,
    DataVenda DATETIME,
);
CREATE TABLE tbdVendaItens (
    VendaItemID INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    VendaID INT,
    Produto VARCHAR(255),
    Quantidade INT,
    PrecoUnitario DECIMAL(10, 2),
    TotalItem DECIMAL(10, 2),
    FOREIGN KEY (VendaID) REFERENCES tbdVenda(VendaID)
);

INSERT INTO tbtfuncionarios (nome, sexo, cpf, endereco, celular, email, data_contrato)
VALUES ('Teste funcionario', 'Masculino', '12345678900', 'Endereço do Funcionário', '(11)1234-5678', 'email@example.com', '2023-10-11 10:00:00');

GO


---criar procedure
CREATE PROCEDURE sp_salvarfunc
    @nome VARCHAR(60),
    @sexo VARCHAR(20),
    @cpf VARCHAR(30),
    @endereco VARCHAR(100),
    @celular VARCHAR(30),
    @email VARCHAR(30),
    @data_contrato DATETIME,
    @mensagem VARCHAR(100) OUTPUT
AS
BEGIN
    IF EXISTS (SELECT * FROM tbtfuncionarios WHERE cpf = @cpf)
    BEGIN
        SET @mensagem = 'Número do CPF ' + @cpf + ' já está cadastrado';
    END
    ELSE
    BEGIN
        INSERT INTO tbtfuncionarios (nome, sexo, cpf, endereco, celular, email, data_contrato)
        VALUES (@nome, @sexo, @cpf, @endereco, @celular, @email, @data_contrato);
        SET @mensagem = 'Funcionário registrado com sucesso';
    END
END
GO

---editar func
CREATE PROCEDURE sp_editarfunc
    @nome VARCHAR(60),
    @sexo VARCHAR(20),
    @cpf VARCHAR(30),
    @endereco VARCHAR(100),
    @celular VARCHAR(30),
    @email VARCHAR(30),
    @data_contrato DATETIME,
    @mensagem VARCHAR(100) OUTPUT
AS
BEGIN
    IF EXISTS (SELECT * FROM tbtfuncionarios WHERE cpf = @cpf)
    BEGIN
        UPDATE tbtfuncionarios 
        SET nome = @nome, sexo = @sexo, endereco = @endereco, celular = @celular, email = @email, data_contrato = @data_contrato
        WHERE cpf = @cpf;
        SET @mensagem = 'Funcionário editado com sucesso';
    END
    ELSE
    BEGIN
        SET @mensagem = 'Funcionário com o CPF ' + @cpf + ' não encontrado';
    END
END
GO


---Excluir funcionario
CREATE PROCEDURE sp_excluirfunc
    @cpf VARCHAR(30),
    @mensagem VARCHAR(100) OUTPUT
AS
BEGIN
    IF EXISTS (SELECT * FROM tbtfuncionarios WHERE cpf = @cpf)
    BEGIN
        DELETE FROM tbtfuncionarios WHERE cpf = @cpf;
        SET @mensagem = 'Funcionário excluído com sucesso';
    END
    ELSE
    BEGIN
        SET @mensagem = 'Funcionário com o CPF ' + @cpf + ' não encontrado';
    END
END
GO

---buscar funcionario por nome
CREATE PROCEDURE sp_buscarfuncnome
    @nome VARCHAR(60),
    @mensagem VARCHAR(100) OUTPUT
AS
BEGIN
    IF EXISTS (SELECT * FROM tbtfuncionarios WHERE nome LIKE '%' + @nome + '%')
    BEGIN
        SELECT * FROM tbtfuncionarios WHERE nome LIKE '%' + @nome + '%';
    END
    ELSE
    BEGIN
        SET @mensagem = 'Nenhum funcionário encontrado com o nome contendo: ' + @nome;
    END
END
GO

---buscar por cpf
CREATE PROCEDURE sp_buscarfuncpf
    @cpf VARCHAR(30),
    @mensagem VARCHAR(100) OUTPUT
AS
BEGIN
    IF EXISTS (SELECT * FROM tbtfuncionarios WHERE cpf LIKE '%' + @cpf + '%')
    BEGIN
        SELECT * FROM tbtfuncionarios WHERE cpf LIKE '%' + @cpf + '%';
    END
    ELSE
    BEGIN
        SET @mensagem = 'Nenhum funcionário encontrado com o CPF contendo: ' + @cpf;
    END
END
GO
-- Salvar Cliente
CREATE PROCEDURE sp_salvarCliente
    @Nome VARCHAR(255),
    @Endereco VARCHAR(255),
    @Cidade VARCHAR(100),
    @UF CHAR(2),
    @mensagem VARCHAR(100) OUTPUT
AS
BEGIN
    IF EXISTS (SELECT * FROM tbdCliente WHERE Nome = @Nome)
    BEGIN
        SET @mensagem = 'Cliente ' + @Nome + ' já está cadastrado';
    END
    ELSE
    BEGIN
        INSERT INTO tbdCliente (Nome, Endereco, Cidade, UF)
        VALUES (@Nome, @Endereco, @Cidade, @UF);
        SET @mensagem = 'Cliente registrado com sucesso';
    END
END
GO


-- Editar Cliente
CREATE PROCEDURE sp_editarCliente
    @Nome VARCHAR(255),
    @Endereco VARCHAR(255),
    @Cidade VARCHAR(100),
    @UF CHAR(2),
    @mensagem VARCHAR(100) OUTPUT
AS
BEGIN
    IF EXISTS (SELECT * FROM tbdCliente WHERE Nome = @Nome)
    BEGIN
        UPDATE tbdCliente
        SET Endereco = @Endereco, Cidade = @Cidade, UF = @UF
        WHERE Nome = @Nome;
        SET @mensagem = 'Cliente editado com sucesso';
    END
    ELSE
    BEGIN
        SET @mensagem = 'Cliente com o nome ' + @Nome + ' não encontrado';
    END
END
GO

-- Excluir Cliente
CREATE PROCEDURE sp_excluirCliente
    @Nome VARCHAR(255),
    @mensagem VARCHAR(100) OUTPUT
AS
BEGIN
    IF EXISTS (SELECT * FROM tbdCliente WHERE Nome = @Nome)
    BEGIN
        DELETE FROM tbdCliente WHERE Nome = @Nome;
        SET @mensagem = 'Cliente excluído com sucesso';
    END
    ELSE
    BEGIN
        SET @mensagem = 'Cliente com o nome ' + @Nome + ' não encontrado';
    END
END
GO

-- Buscar Cliente por Nome
CREATE PROCEDURE sp_buscarClienteNome
    @Nome VARCHAR(255),
    @mensagem VARCHAR(100) OUTPUT
AS
BEGIN
    IF EXISTS (SELECT * FROM tbdCliente WHERE Nome LIKE '%' + @Nome + '%')
    BEGIN
        SELECT * FROM tbdCliente WHERE Nome LIKE '%' + @Nome + '%';
    END
    ELSE
    BEGIN
        SET @mensagem = 'Nenhum cliente encontrado com o nome contendo: ' + @Nome;
    END
END
GO

