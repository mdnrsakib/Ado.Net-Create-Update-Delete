Create table employees
(
	employeeid int primary key,
	[name] nvarchar(40) not null,
	joiningdate date not null,
	salary money not null,
	[address] nvarchar(50) not null,
	phone nvarchar(30) not null,
	isWorking bit
)
Create table projects
(
	projectid int primary key,
	projectname nvarchar(40) not null,
	budget money not null,
	isRunning bit,
	employeeid int references employees(employeeid)
)