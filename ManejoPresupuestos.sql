create database ManejoPresupuesto 
drop database ManejoPresupuesto
drop table Transacciones

/*Crear tabla Transacciones*/

create table Transacciones(
Id int identity(1,1) primary key,
UsuarioId int not null,
FechaTransaccion Date not null,
Monto decimal(18,2) not null,
Nota varchar(1000) ,
CuentaId int not null,
CategoriaId int not null
);

select * from Transacciones
select * from Cuentas

alter table Transacciones add  CuentaId int not null

delete Transacciones

/*Crear tabla TiposOperaciones*/

create table TiposOperaciones(
Id int identity(1,1) primary key ,
Descripcion varchar(50)
);

insert into TiposOperaciones(Descripcion) values ('Ingreso')
select * from TiposOperaciones
update TiposOperaciones set Descripcion = 'Gasto' where id = 2

delete TiposOperaciones where id = 1 and id = 2

insert into TiposOperaciones(Descripcion) values ('Gastos')
select * from TiposOperaciones
select * from Transacciones
select * from Cuentas
insert into Transacciones(UsuarioId, Monto, FechaTransaccion, Nota, TipoOperacionId, CuentaId) values (1,65000, '2021-08-16', 'Voy a ahorrar 20000', 2, 2)

/*Crear tabla de Cuentas y TiposCuentas*/
create table TiposCuentas (
Id int identity(1,1) primary key,
Nombre varchar(100) not null,
UsuarioId int not null,
Orden int not null
);

alter table TiposCuentas add constraint Fk_TiposCuentas_Usuarios foreign key(UsuarioId) references Usuarios(Id)
insert into TiposCuentas(Nombre, UsuarioId, Orden) values ('juan@gmail.com', 1, 2)


create table Cuentas (
Id int identity(1,1) primary key,
Nombre varchar(100) not null,
TipoCuentaId int not null,
Balance decimal(18,2) not null,
Descripcion varchar(1000)
);

/*Crear tabla de usuarios, un HASH es como cifrar las contraseñas*/
create table Usuarios(
Id int identity(1,1) primary key,
Email varchar(256) not null,
EmailNormalizado varchar(256) not null,
PasswordHash varchar(1000) not null,
);

/*Crear tabla de categorias*/
create table Categorias(
Id int identity(1,1) primary key,
Nombre varchar(100) not null,
TipoOperacionId int not null,
UsuarioId int not null,
);

alter table Categorias add constraint Fk_Categorias_TiposOperaciones foreign key (TipoOperacionId) references TiposOperaciones(Id)
alter table Categorias add constraint Fk_Categorias_Usuarios foreign key (UsuarioId) references Usuarios(Id)




insert into Usuarios(Email, EmailNormalizado, PasswordHash) values ('juan@gmail.com', 'JUAN@gmail.com', '123')
alter table Transacciones add constraint Fk_Transacciones_Usuarios foreign key(UsuarioId) references Usuarios (Id)

drop table Usuarios

insert into Cuentas(Nombre, TipoCuentaId, Balance, Descripcion) values ('Carlos cuentas', 2, 5200, 'Hola')
insert into TiposCuentas(Nombre, UsuarioId, Orden) values ('MasterCard', 1, 2)
 
/*Seleccionar todos los datos de la tabla*/
SELECT * from Transacciones;

/*Insertar datos en la tabla*/
insert into Transacciones(UsuarioId, Monto, FechaTransaccion, Nota, TipoOperacionId) values ('Andres',65000, '2021-06-25', 'Voy a ahorrar 20000', 1)
insert into Transacciones(UsuarioId, Monto, FechaTransaccion, TipoOperacionId) values ('Andres',5000, '2021-08-16', 1)


/*Actualizar datos de la tabla*/
UPDATE Transacciones set Nota = 'Nota actualizada', UsuarioId = 'Felipe' where Id = 9
UPDATE Transacciones set UsuarioId = 'Liliana' where id = 1
/*Borrar datos de la tabla*/
delete from Transacciones where Id = 2

/*El operador in permite seleccionar los datos que coincida en la busque que se le manda dentro de los parentesis, tambien se le puede decir que  no muestr los valores*/
select * from Transacciones where UsuarioId in ('Pepe', 'Carlos')
select * from Transacciones where Monto in ('25000', '200')
select * from Transacciones where Monto not in ('150099', '200')

/*El operador like nos permite seleccionar los valores que coincidan con la busca, si se dejan los dos % busca por la palabra o letra que se le pase, si se 
deja el % al inicio quiere decir que debe comenzar por la palabra o la letra que se le pase y si se le deja al final el %, quiere decir que el resultado a buscar
debe terminar en la letra  o la palabra que se le pase, tambien se le puede decir que no busque*/
select * from Transacciones where UsuarioId like '%pep%'
select * from Transacciones where UsuarioId not like '%Rafa%'
select * from Transacciones where UsuarioId  like 'R%'
select * from Transacciones where UsuarioId  like '%e'
select * from Transacciones where UsuarioId  like '%el' and Nota not like '%Esta es%'

/*Operadores logicos or y and para combinar condiciones*/
select * from Transacciones where Monto = 5001 or Monto = 500 and UsuarioId = 'Rafael'

/*Seleccionar con un dato en especifico*/
delete from Transacciones where id = 18

/*Filtrar por fechas se puede hacer directamente o por rangos o por dias, años y mes*/
select * from Transacciones where FechaTransaccion =  '2023-04-17'
select * from Transacciones where FechaTransaccion > '2022-05-17' and FechaTransaccion < '2023-04-17'
select * from Transacciones where YEAR(FechaTransaccion) = 2022
select * from Transacciones where MONTH(FechaTransaccion) = 5
select * from Transacciones where DAY(FechaTransaccion) = 17

/*Operador between osea entre*/
select * from Transacciones where FechaTransaccion BETWEEN '2022-05-17' and  '2023-04-17'
select * from Transacciones where Monto BETWEEN 14500 and  25000
select * from Transacciones where Monto NOT BETWEEN 14500 and  25000

/*Operador top, esto funciona para traer los primeros n elemento de un query*/
select TOP 2 * FROM Transacciones
select TOP 1 * FROM Transacciones where Monto > 25000
select top 25 percent * from Transacciones

/*Group By y funcion sum*/
select sum(Monto) as suma, UsuarioId from Transacciones group by UsuarioId
select sum(Monto) as suma, MONTH(FechaTransaccion) as Mes from Transacciones group by MONTH(FechaTransaccion)
select sum(Monto) as suma, MONTH(FechaTransaccion) as Mes from Transacciones where UsuarioId= 'Liliana' group by MONTH(FechaTransaccion)
select sum(Monto) as suma, UsuarioId, TipoTransaccionId  from Transacciones group by UsuarioId, TipoTransaccionId

/*Funcion conteo count*/
select count(*) as conteo from Transacciones
select count(Nota) as conteo from Transacciones
select count(*) as conteo from Transacciones where Nota is not null
select count(*) as conteo, UsuarioId from Transacciones group by UsuarioId
select count(*) as conteo, UsuarioId from Transacciones where UsuarioId = 'penelope' group by UsuarioId

/*Funcion promedio AVG*/
select count(*) as conteo, UsuarioId, AVG(Monto) as promedio from Transacciones where UsuarioId = 'penelope' group by UsuarioId
select count(*) as conteo, UsuarioId, AVG(Monto) as promedio from Transacciones group by UsuarioId




/*La llave foranea es una restriccion, que permite relacionar unas tablas con otras, solo funciona si se insertan
valores en el campo que fue relacionado con la llave foranea y que coincida con el dato de la llave foranea*/

/*Inner join, permite realizar un query para combinar los datos de dos o mas tablas, on permite decir que columna se va a combinar con la otra columna de la otra tabla
se utiliza el nombre de la tabla con el campo cuando hay campos iguales en ambas tablas, osea columnas ambiguas ejemplo: Transacciones.(nombre de la columna)
*/
select Transacciones.Id, UsuarioId, Monto, Nota, TipoOperacionId, TiposOperaciones.descripcion from Transacciones inner join TiposOperaciones on Transacciones.TipoOperacionId = TiposOperaciones.Id
select Transacciones.Id, UsuarioId, Monto, Nota, TiposOperaciones.descripcion from Transacciones inner join TiposOperaciones on Transacciones.TipoOperacionId = TiposOperaciones.Id


/*Procedimiento almacenado, nos permite encapsular los querys en una unidad para utilizarlos en consultas, insertar, actualizar y borrar datos, se le pueden pasar parametros
nos permite reutilizar querys y centralizar la informacion
*/

create procedure Transacciones_SelectConTipoOperacion
as
begin

	set nocount on;
	select Transacciones.Id, UsuarioId, Monto, Nota, TiposOperaciones.descripcion from Transacciones inner join TiposOperaciones on Transacciones.TipoOperacionId = TiposOperaciones.Id

end
go

/*Ejecutar prodecimiento almacenado*/
exec Transacciones_SelectConTipoOperacion

/*Alterar o actualizar un procedimiento almacenado*/

alter procedure Transacciones_SelectConTipoOperacion
as
begin

	set nocount on;
	select Transacciones.Id, UsuarioId, Monto, Nota, TiposOperaciones.descripcion from Transacciones inner join TiposOperaciones on Transacciones.TipoOperacionId = TiposOperaciones.Id order by UsuarioId desc

end

/*Procedimientos almacenados con parametros
para crear el parametros se escribe antes del as y se usa @nombre parametro y el tipo de dato
*/
create procedure Transacciones_SelectConTipoOperacion_ConParametro
	@fecha date
as
begin

	set nocount on;
	select Transacciones.Id, UsuarioId, Monto, Nota, TiposOperaciones.descripcion from Transacciones inner join TiposOperaciones on Transacciones.TipoOperacionId = TiposOperaciones.Id where FechaTransaccion = @fecha order by UsuarioId desc

end
go

exec Transacciones_SelectConTipoOperacion_ConParametro '2021-06-25'
select * from Transacciones

/*Procedimiento almacenado con varios parametros*/
create procedure Transacciones_Insertar_Varios_Parametros
	@UsuarioId varchar(450),
	@FechaTransaccion date,
	@Monto decimal(18,2),
	@Nota varchar(1000) = null,
	@TipoOperacionId int
as
begin

	set nocount on;
	insert into Transacciones(UsuarioId, FechaTransaccion, Monto, Nota, TipoOperacionId) values (@UsuarioId, @FechaTransaccion, @Monto, @Nota, @TipoOperacionId)


end
go

exec Transacciones_Insertar_Varios_Parametros 'Carlos','2021-11-07', 250000, 'Nota ejemplo', 1
exec Transacciones_Insertar_Varios_Parametros 'Carlos','2021-11-07', 250000, null, 1

select * from Transacciones

select * from TiposCuentas
select * from Cuentas

delete from TiposCuentas where Id = 7
delete from Cuentas where Id = 2

 

SELECT Cuentas.Id, Cuentas.Nombre, Cuentas.Balance, tc.Nombre AS TipoCuenta  FROM Cuentas INNER JOIN TiposCuentas tc ON tc.Id = Cuentas.TipoCuentaId WHERE tc.UsuarioId = @UsuarioId ORDER BY tc.Orden


create procedure Transacciones_Insertar
	@UsuarioId int,
	@FechaTransaccion date, 
	@Monto decimal(18,2),
	@CategoriaId int,
	@CuentaId int,
	@Nota nvarchar(1000) = NULL

as
begin

	set nocount on;
	insert into Transacciones(UsuarioId, FechaTransaccion, Monto, CategoriaId,CuentaId, Nota) Values(@UsuarioId, @FechaTransaccion, abs(@Monto), @CategoriaId, @CuentaId, @Nota)
	update Cuentas Set Balance += @Monto Where Id = @CuentaId

	select SCOPE_IDENTITY();

end
go

create procedure Transacciones_Actualizar
	@Id int,
	@FechaTransaccion datetime,
	@Monto decimal(18,2),
	@MontoAnterior decimal(18,2),
	@CuentaId int,
	@CuentaAnteriorId int,
	@CategoriaId int,
	@Nota nvarchar(1000) = NULL
as 

begin
	set nocount on;

	--Revertir transaccion anterior
	UPDATE Cuentas SET Balance -= @MontoAnterior WHERE Id = @CuentaAnteriorId;

	-- Realizar nueva transaccion
	UPDATE Cuentas SET Balance += @Monto WHERE Id = @CuentaId;

	UPDATE Transacciones SET Monto = ABS(@Monto), FechaTransaccion = @FechaTransaccion, CategoriaId = @CategoriaId, CuentaId = @CuentaId, Nota = @Nota WHERE Id = @Id

end
go

create procedure Transacciones_Borrar
	@Id int

as

begin

	set nocount on;

	declare @Monto decimal(18,2);
	declare @CuentaId int;
	declare @TipoOperacionId int;

	SELECT @Monto = Monto, @CuentaId = CuentaId, @TipoOperacionId = cat.TipoOperacionId FROM Transacciones inner join Categorias cat ON cat.Id = Transacciones.CategoriaId WHERE Transacciones.Id = @Id;

	declare @FactorMultiplicativo int = 1;

	If(@TipoOperacionId = 2)
		SET @FactorMultiplicativo = -1


	SET @Monto = @Monto * @FactorMultiplicativo;

	UPDATE Cuentas SET Balance -= @Monto WHERE Id = @CuentaId
	
	DELETE Transacciones WHERE Id = @Id;


end

go


declare @fechaInicio date = '2022-09-01';
declare @fechaFin date = '2022-09-30';
declare @usuarioId int = 1;

SELECT DATEDIFF(d, @fechaInicio, FechaTransaccion) / 7 + 1 as Semana, SUM(Monto) AS Monto 
FROM Transacciones inner join Categorias cat on cat.id = Transacciones.CategoriaId WHERE Transacciones.UsuarioId = @usuarioId AND FechaTransaccion 
BETWEEN @fechaInicio and @fechaFin GROUP BY DATEDIFF(d, @fechaInicio, FechaTransaccion) / 7, cat.TipoOperacionId

DECLARE @usuarioId int = 1;
DECLARE @Año int = 2022;

SELECT MONTH(FechaTransaccion) as Mes, SUM(Monto) as Monto, cat.TipoOperacionId 
FROM Transacciones INNER JOIN Categorias cat ON cat.Id = Transacciones.CategoriaId 
WHERE Transacciones.UsuarioId = @usuarioId AND  YEAR(FechaTransaccion) = @Año GROUP BY MONTH(FechaTransaccion), cat.TipoOperacionId

select * from  Usuarios

create procedure CrearDatosUsuarioNuevo
	@UsuarioId int
as
begin

	set nocount on;

	declare @Efectivo nvarchar(50) = 'Efectivo';
	declare @CuentaDeBanco nvarchar(50) = 'Cuentas de banco';
	declare @tarjetas nvarchar(50) = 'Tarjetas';

	insert into TiposCuentas(Nombre, UsuarioId, Orden) values (@Efectivo, @UsuarioId, 1), (@CuentaDeBanco, @UsuarioId, 2), (@tarjetas, @UsuarioId, 3);

	insert into Cuentas(Nombre, Balance, TipoCuentaId)
	select Nombre, 0, Id From TiposCuentas Where UsuarioId = @UsuarioId;

	insert into Categorias(Nombre, TipoOperacionId, UsuarioId)
	values
	('Libros', 2, @UsuarioId),
	('Salario', 1, @UsuarioId),
	('Comida', 2, @UsuarioId),
	('Mesada', 1, @UsuarioId),
	('Arriendo', 2, @UsuarioId),
	('Mercado', 2, @UsuarioId),
	('Diversión', 2, @UsuarioId)

end
go

select * from Usuarios