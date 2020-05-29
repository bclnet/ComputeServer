-- sys_database_principal
create table sys_database_principal
(
    name                varchar(30) not null,
    principal_id        integer primary key AUTOINCREMENT,
    sid                 blob null,
    type                char(2) not null,
    default_schema_name varchar(30) null,
    create_date         datetime not null default (datetime()),
    modify_date         datetime not null default (datetime())
);
GO
INSERT INTO sys_database_principal (name, sid, type, default_schema_name)
VALUES 
    ('public', null, 'R', null),
    ('dbo', 0x01, 'S', 'dbo'),
    ('guest', 0x00, 'S', 'guest'),
    ('sys', 0x00, 'S', 'sys'),
    ('db_owner', 0x0, 'R', null);
GO

-- sys_database_role_member
create table sys_database_role_member
(
    role_principal_id   integer,
    member_principal_id integer,
    foreign key (role_principal_id) references sys_database_principal (principal_id) on delete cascade, 
    foreign key (member_principal_id) references sys_database_principal (principal_id) on delete cascade
);
GO
INSERT INTO sys_database_role_member (role_principal_id, member_principal_id)
VALUES
    ((SELECT principal_id FROM sys_database_principal WHERE name = 'db_owner' And type = 'R'), (SELECT principal_id FROM sys_database_principal WHERE name = 'dbo' And type = 'S'));
GO

-- sys_schema
create table sys_schema
(
    name                varchar(30) not null,
    schema_id           integer primary key AUTOINCREMENT,
    princial_id         integer not null,
    foreign key (princial_id) references sys_database_principal (principal_id) 
);
GO
INSERT INTO sys_schema (name, princial_id)
VALUES 
    ('dbo', (SELECT principal_id FROM sys_database_principal WHERE name = 'dbo' And type = 'S')),
    ('guest', (SELECT principal_id FROM sys_database_principal WHERE name = 'guest' And type = 'S')),
    ('sys', (SELECT principal_id FROM sys_database_principal WHERE name = 'sys' And type = 'S'));
GO

-- sys_object
create table sys_object
(
    name                varchar(30) not null,
    object_id           integer primary key AUTOINCREMENT,
    schema_id           integer null,
    type                char(2) not null,
    create_date         datetime not null default (datetime()),
    modify_date         datetime not null default (datetime()),
    foreign key (schema_id) references sys_schema (schema_id) 
);
GO