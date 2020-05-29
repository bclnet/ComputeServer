-- sys_database
create table sys_database
(
    name                varchar(30) not null,
    database_id         integer primary key AUTOINCREMENT,
    create_date         datetime not null default (datetime()),
    recovery_model      tinyint not null
);
GO
INSERT INTO sys_database (name, recovery_model)
VALUES 
    ('master', 0);
GO

-- sys_database_file
create table sys_database_file
(
    file_id             integer primary key AUTOINCREMENT,
    database_id         integer not null,
    type                tinyint not null,
    name                varchar(30) not null,
    physical_name       varchar(256) not null,
    state               tinyint not null,
    foreign key (database_id) references sys_database_principal (sys_database) on delete cascade
);
GO
INSERT INTO sys_database_file (database_id, type, name, physical_name, state)
VALUES 
    ((SELECT database_id FROM sys_database WHERE name = 'master'), 0, 'master', '${folder}\master.db', 0);
GO--${folder}

-- sys_server
create table sys_server
(
    server_id           integer primary key AUTOINCREMENT,
    name                varchar(30) not null,
    provider            varchar(30) not null,
    data_source         varchar(256) not null,
    modify_date         datetime not null
);
GO

-- sys_server_principal
create table sys_server_principal
(
    name                varchar(30) not null,
    principal_id        integer primary key AUTOINCREMENT,
    sid                 blob not null,
    type                char(2) not null,
    is_disabled         tinyint not null,
    create_date         datetime not null default (datetime()),
    modify_date         datetime not null default (datetime()),
    default_database_name varchar(30) null,
    default_language_name varchar(30) null
);
GO
INSERT INTO sys_server_principal (name, sid, type, is_disabled,  default_database_name, default_language_name)
VALUES 
    ('sa', 0x01, 'S', 0, 'master', 'us_english'),
    ('public', 0x02, 'R', 0, null, null),
    ('sysadmin', 0x03, 'R', 0, null, null);
GO

-- sys_server_role_member
create table sys_server_role_member
(
    role_principal_id   integer,
    member_principal_id integer,
    foreign key (role_principal_id) references sys_server_principal (principal_id) on delete cascade, 
    foreign key (member_principal_id) references sys_server_principal (principal_id) on delete cascade
);
GO
INSERT INTO sys_server_role_member (role_principal_id, member_principal_id)
VALUES
    ((SELECT principal_id FROM sys_server_principal WHERE name = 'sysadmin' And type = 'R'), (SELECT principal_id FROM sys_server_principal WHERE name = 'sa' And type = 'S'));
GO

-- sys_server_permission
create table sys_server_permission
(
    class               integer,
    grantee_principal_id integer,
    grantor_principal_id integer,
    type                char(2) not null,
    state               char(1) not null,
    foreign key (grantee_principal_id) references sys_server_principal (principal_id) on delete cascade, 
    foreign key (grantor_principal_id) references sys_server_principal (principal_id) on delete cascade
);
GO