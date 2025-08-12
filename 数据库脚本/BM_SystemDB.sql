USE BM_SystemDB;
GO

/*项目基础信息*/
--角色信息表
IF OBJECT_ID ('dbo.SA_Role') IS NOT NULL
	DROP TABLE dbo.SA_Role
GO
CREATE TABLE dbo.SA_Role
(
	RoleID	INT IDENTITY(1,1)	NOT NULL, --自增ID
	RoleGUID	 VARCHAR(36) NOT NULL, --角色GUID
	RoleName	 NVARCHAR(100) NOT NULL, --角色名称
	RoleType     VARCHAR(20) NOT NULL, --角色类型：sa-超级管理员，other-其它
	RoleNote	 NVARCHAR(200) NULL, --备注
	Create_Time	 DATETIME NOT NULL, --创建时间
	CONSTRAINT PK_SA_Role PRIMARY KEY (RoleID)
)
GO
INSERT INTO dbo.SA_Role(RoleGUID,RoleName,RoleType,RoleNote,Create_Time) VALUES (NEWID(), '超级管理员', 'sa', '系统默认添加账号，拥有所有权限',GETDATE());
GO
--设置唯一键
ALTER TABLE dbo.SA_Role ADD CONSTRAINT UQ_SA_Role UNIQUE (RoleName);
GO
-- 创建索引
CREATE INDEX IX_SA_Role ON dbo.SA_Role (RoleGUID,RoleName);
GO

--系统账号信息表
IF OBJECT_ID ('dbo.SA_User') IS NOT NULL
	DROP TABLE dbo.SA_User
GO
CREATE TABLE dbo.SA_User
(
	UserID	INT IDENTITY(1,1)	NOT NULL, --自增ID
	UserGUID      VARCHAR(36) NOT NULL, --用户GUID
	UserName      NVARCHAR(100) NOT NULL, --用户名
	UserPassWord  VARCHAR(36) NOT NULL, --用户密码（MD5加密）
	UserStatus    INT NOT NULL, --状态：-1-禁用，0-启用
	UserSex       VARCHAR(2) NULL, --性别：M-男，F-女
	UserRoleID  INT NOT NULL, --角色ID
	Phone         VARCHAR(15) NULL, --联系电话
	Email         VARCHAR(100) NULL, --邮箱账号
	Create_Time	  DATETIME NOT NULL, --创建时间
	CONSTRAINT PK_SA_User PRIMARY KEY (UserID)
)
GO
--设置唯一键
ALTER TABLE dbo.SA_User ADD CONSTRAINT UQ_SA_User UNIQUE (UserName);
GO
-- 创建索引
CREATE INDEX IX_SA_User ON dbo.SA_User (UserGUID,UserName,UserStatus,UserSex);
GO
INSERT INTO dbo.SA_User(UserGUID,UserName,UserPassWord,UserSex,UserRoleID,UserStatus,Phone,Email,Create_Time) VALUES (NEWID(), 'sa', 'E10ADC3949BA59ABBE56E057F20F883E', '', 1,0,NULL,NULL,GETDATE());
GO

--菜单信息表
IF OBJECT_ID ('dbo.SA_Module') IS NOT NULL
	DROP TABLE dbo.SA_Module
GO
CREATE TABLE dbo.SA_Module
(
	ModuleID	 INT NOT NULL, --菜单ID
    ModuleName	 NVARCHAR(100) NOT NULL, --菜单名称
	ParentID	 INT NOT NULL, --上级菜单ID
	Link	     VARCHAR(100) NULL, --链接地址
	OrderNo	     INT NOT NULL, --排序
	IsDisplay	 VARCHAR(2) NOT NULL, --是否显示：Y-显示，N-不显示
	IsFirstMenu	 VARCHAR(2) NOT NULL, --是否一级菜单：Y-是，N-不是
	Flag	     INT NULL, --标识
	Create_Time	 DATETIME NOT NULL, --创建时间
	CONSTRAINT PK_SA_Module PRIMARY KEY (ModuleID)
)
GO
--设置唯一键
ALTER TABLE dbo.SA_Module ADD CONSTRAINT UQ_SA_Module UNIQUE (ModuleName);
GO
-- 创建索引
CREATE INDEX IX_SA_Module ON dbo.SA_Module (ModuleID,ModuleName,ParentID);
GO

--角色菜单权限表
IF OBJECT_ID ('dbo.SA_RolePermission') IS NOT NULL
	DROP TABLE dbo.SA_RolePermission
GO
CREATE TABLE dbo.SA_RolePermission
(
	RoleGUID	 VARCHAR(36) NOT NULL, --角色GUID
	ModuleID	INT NOT NULL, --菜单ID
	CONSTRAINT PK_SA_RolePermission PRIMARY KEY (RoleGUID,ModuleID)
)
GO
-- 创建索引
CREATE INDEX IX_SA_RolePermission ON dbo.SA_RolePermission (RoleGUID);
GO

--部门信息表
IF OBJECT_ID ('dbo.SA_Department') IS NOT NULL
	DROP TABLE dbo.SA_Department
GO
CREATE TABLE dbo.SA_Department
(
	DepartmentID	INT IDENTITY(1,1)	NOT NULL, --自增ID
	DepartmentGUID      VARCHAR(36) NOT NULL, --部门GUID
	DepartmentName      NVARCHAR(100) NOT NULL, --部门名称
	DepartmentNote      NVARCHAR(200) NULL, --备注
	Create_Time	  DATETIME NOT NULL, --创建时间
	CONSTRAINT PK_SA_Department PRIMARY KEY (DepartmentID)
)
GO
--设置唯一键
ALTER TABLE dbo.SA_Department ADD CONSTRAINT UQ_SA_Department UNIQUE (DepartmentName);
GO
-- 创建索引
CREATE INDEX IX_SA_Department ON dbo.SA_Department (DepartmentGUID,DepartmentName);
GO

/*系统设置*/
--系统参数表
IF OBJECT_ID ('dbo.SA_Parameter') IS NOT NULL
	DROP TABLE dbo.SA_Parameter
GO
CREATE TABLE dbo.SA_Parameter
(
	PID		 	    INT IDENTITY(1,1) NOT NULL,	--自增ID
	ParameterName	VARCHAR(50) NOT NULL, --键名
	ParameterValue	NVARCHAR(100) NOT NULL, --键值
	ParameterNote	NVARCHAR(200) NOT NULL, --备注
	IsDisplay	 VARCHAR(2) NOT NULL, --是否显示：Y-显示，N-不显示
	Create_Time	 DATETIME NOT NULL, --创建时间
	CONSTRAINT PK_SA_Parameter PRIMARY KEY (PID)
)
GO
--设置唯一键
ALTER TABLE dbo.SA_Parameter ADD CONSTRAINT UQ_SA_Parameter UNIQUE (ParameterName);
GO
-- 创建索引
CREATE INDEX IX_SA_Parameter ON dbo.SA_Parameter (ParameterName);
GO
INSERT INTO dbo.SA_Parameter(ParameterName,ParameterValue,ParameterNote,IsDisplay,Create_Time) VALUES ('project_name', '后台管理系统','系统名称', 'Y',GETDATE());
INSERT INTO dbo.SA_Parameter(ParameterName,ParameterValue,ParameterNote,IsDisplay,Create_Time) VALUES ('company_name', 'XXXX科技有限公司','公司名称', 'Y',GETDATE());
GO

/*日志信息*/
--用户登录日志表
IF OBJECT_ID ('dbo.Log_Login') IS NOT NULL
    DROP TABLE dbo.Log_Login
GO
CREATE TABLE dbo.Log_Login
(
    LogID           INT IDENTITY(1,1) NOT NULL, --日志ID，自增
    UserID        INT NOT NULL,     --用户ID
	UserName        NVARCHAR(100) NOT NULL, --用户名
	UserRoleID    INT NOT NULL, --用户角色ID
	UserRoleName	NVARCHAR(100) NOT NULL, --用户角色名称
    Operation_Time  DATETIME NOT NULL,         --登录时间
    Operation_IP    VARCHAR(50) NOT NULL,     --登录IP
    Operation_Note  NVARCHAR(200) NULL,        --备注
    CONSTRAINT PK_Log_Login PRIMARY KEY (LogID)
)
GO
-- 创建索引
CREATE INDEX IX_Log_Login ON dbo.Log_Login (UserID, Operation_Time, Operation_IP);
GO

--数据操作日志表
IF OBJECT_ID ('dbo.Log_Operation') IS NOT NULL
	DROP TABLE dbo.Log_Operation
GO
CREATE TABLE dbo.Log_Operation
(
	LogID			INT IDENTITY(1,1) NOT NULL,	--日志ID，自增
	Object_Name		VARCHAR(50) NOT NULL, --操作对象
	UserID	    INT NOT NULL, --操作用户ID
	UserName        NVARCHAR(100) NOT NULL, --操作用户名
	UserRoleID    INT NOT NULL, --操作用户角色ID
	UserRoleName	NVARCHAR(100) NOT NULL, --操作用户角色名称
	Operation_Time	DATETIME NOT NULL, --操作时间
	Operation_Type	VARCHAR(50) NOT NULL, --操作类型（例如：Add、Update、Delete、Import）
	Operation_IP	VARCHAR(50) NOT NULL, --操作IP
	Operation_Note	NVARCHAR(200) NULL, --备注
	CONSTRAINT PK_Log_Operation PRIMARY KEY (LogID)
)
GO
-- 创建索引
CREATE INDEX IX_Log_Operation ON dbo.Log_Operation (Object_Name,UserID,UserRoleID,Operation_Time,Operation_Type,Operation_IP);
GO

--数据修改痕迹日志表
IF OBJECT_ID ('dbo.Log_Trace') IS NOT NULL
	DROP TABLE dbo.Log_Trace
GO
CREATE TABLE dbo.Log_Trace
(
	LogID			INT IDENTITY(1,1) NOT NULL,	--日志ID，自增
	Object_Type		NVARCHAR(50) NOT NULL, --操作对象类型（例如：用户：user）
	Object_GUID		VARCHAR(36) NOT NULL, --操作对象ID
	Object_Name		NVARCHAR(100) NOT NULL, --操作对象名
	Object_Field    VARCHAR(50) NULL, --操作对象属性（例如：用户名称：UserName）
	Old_Value		NVARCHAR(300) NULL, --旧值
	New_Value		NVARCHAR(300) NULL, --新值
	UserID	    INT NOT NULL, --操作用户ID
	UserName        NVARCHAR(100) NOT NULL, --操作用户名
	UserRoleID    INT NOT NULL, --操作用户角色ID
	UserRoleName	NVARCHAR(100) NOT NULL, --操作用户角色名称
	Operation_Time	DATETIME NOT NULL, --操作时间
	Operation_Type	VARCHAR(50) NOT NULL, --操作类型（例如：Add、Update、Delete、Import）
	Operation_IP	VARCHAR(50) NOT NULL, --操作IP
	Operation_Note	NVARCHAR(200) NULL, --备注
	CONSTRAINT PK_Log_Trace PRIMARY KEY (LogID)
)
GO
-- 主查询索引（键长度172字节）
CREATE INDEX IX_Log_Trace_Main ON dbo.Log_Trace (Object_Type, Object_GUID) INCLUDE (UserID, UserName, Operation_Time);
GO
-- 审计查询索引（键长度272字节）
CREATE INDEX IX_Log_Trace_Audit ON dbo.Log_Trace (Operation_Type, Operation_Time) INCLUDE (Object_Name, UserName);
GO




/*视图*/
--获取账号信息
IF OBJECT_ID ('dbo.SA_User_v') IS NOT NULL
	DROP VIEW dbo.SA_User_v
GO
CREATE VIEW [dbo].[SA_User_v] AS 
SELECT a.*,b.RoleName
FROM dbo.SA_User a
LEFT JOIN dbo.SA_Role b ON a.UserRoleID=b.RoleID
GO

--获取用户登录日志信息
IF OBJECT_ID ('dbo.Log_Login_v') IS NOT NULL
	DROP VIEW dbo.Log_Login_v
GO
CREATE VIEW [dbo].[Log_Login_v] AS 
SELECT a.LogID,a.UserID,a.UserRoleID,a.Operation_Time,a.Operation_IP,a.Operation_Note,
	ISNULL(b.UserName,a.UserName) AS UserName,ISNULL(b.RoleName,a.UserRoleName) AS UserRoleName
FROM dbo.Log_Login a
LEFT JOIN dbo.SA_User_v b ON a.UserID=b.UserID
GO

--获取操作日志信息
IF OBJECT_ID ('dbo.Log_Operation_v') IS NOT NULL
	DROP VIEW dbo.Log_Operation_v
GO
CREATE VIEW [dbo].[Log_Operation_v] AS 
SELECT a.LogID,a.Object_Name,a.UserID,a.UserRoleID,a.Operation_Time,a.Operation_Type,
	a.Operation_Note,a.Operation_IP,ISNULL(b.UserName,a.UserName) AS UserName,ISNULL(b.RoleName,a.UserRoleName) AS UserRoleName
FROM dbo.Log_Operation a
LEFT JOIN dbo.SA_User_v b ON a.UserID=b.UserID
GO

--获取数据修改痕迹日志信息
IF OBJECT_ID ('dbo.Log_Trace_v') IS NOT NULL
	DROP VIEW dbo.Log_Trace_v
GO
CREATE VIEW [dbo].[Log_Trace_v] AS 
--角色信息日志
SELECT a.LogID,a.Object_Type,a.Object_GUID,ISNULL(c.RoleName,a.Object_Name) AS Object_Name,a.Object_Field,a.Old_Value,a.New_Value,a.UserID,a.UserRoleID,a.Operation_Time,
	a.Operation_Type,a.Operation_Note,a.Operation_IP,ISNULL(b.UserName,a.UserName) AS UserName,ISNULL(b.RoleName,a.UserRoleName) AS UserRoleName
FROM dbo.Log_Trace a
LEFT JOIN dbo.SA_User_v b ON a.UserID=b.UserID
LEFT JOIN dbo.SA_Role c ON a.Object_GUID=c.RoleGUID
WHERE a.Object_Type='role'
UNION
--账号信息日志
SELECT a.LogID,a.Object_Type,a.Object_GUID,ISNULL(c.UserName,a.Object_Name) AS Object_Name,a.Object_Field,a.Old_Value,a.New_Value,a.UserID,a.UserRoleID,a.Operation_Time,
	a.Operation_Type,a.Operation_Note,a.Operation_IP,ISNULL(b.UserName,a.UserName) AS UserName,ISNULL(b.RoleName,a.UserRoleName) AS UserRoleName
FROM dbo.Log_Trace a
LEFT JOIN dbo.SA_User_v b ON a.UserID=b.UserID
LEFT JOIN dbo.SA_User c ON a.Object_GUID=c.UserGUID
WHERE a.Object_Type='user'
UNION
--部门信息日志
SELECT a.LogID,a.Object_Type,a.Object_GUID,ISNULL(c.DepartmentName,a.Object_Name) AS Object_Name,a.Object_Field,a.Old_Value,a.New_Value,a.UserID,a.UserRoleID,a.Operation_Time,
	a.Operation_Type,a.Operation_Note,a.Operation_IP,ISNULL(b.UserName,a.UserName) AS UserName,ISNULL(b.RoleName,a.UserRoleName) AS UserRoleName
FROM dbo.Log_Trace a
LEFT JOIN dbo.SA_User_v b ON a.UserID=b.UserID
LEFT JOIN dbo.SA_Department c ON a.Object_GUID=c.DepartmentGUID
WHERE a.Object_Type='department'
UNION
--学历信息日志
SELECT a.LogID,a.Object_Type,a.Object_GUID,ISNULL(c.EducationalBackgroundName,a.Object_Name) AS Object_Name,a.Object_Field,a.Old_Value,a.New_Value,a.UserID,a.UserRoleID,a.Operation_Time,
	a.Operation_Type,a.Operation_Note,a.Operation_IP,ISNULL(b.UserName,a.UserName) AS UserName,ISNULL(b.RoleName,a.UserRoleName) AS UserRoleName
FROM dbo.Log_Trace a
LEFT JOIN dbo.SA_User_v b ON a.UserID=b.UserID
LEFT JOIN dbo.SA_EducationalBackground c ON a.Object_GUID=c.EducationalBackgroundGUID
WHERE a.Object_Type='educationalbackground'
UNION
--参数配置信息日志
SELECT a.LogID,a.Object_Type,a.Object_GUID,ISNULL(c.ParameterName,a.Object_Name) AS Object_Name,a.Object_Field,a.Old_Value,a.New_Value,a.UserID,a.UserRoleID,a.Operation_Time,
	a.Operation_Type,a.Operation_Note,a.Operation_IP,ISNULL(b.UserName,a.UserName) AS UserName,ISNULL(b.RoleName,a.UserRoleName) AS UserRoleName
FROM dbo.Log_Trace a
LEFT JOIN dbo.SA_User_v b ON a.UserID=b.UserID
LEFT JOIN dbo.SA_Parameter c ON a.Object_GUID=c.ParameterName
WHERE a.Object_Type='parameter'
GO


--插入菜单数据
TRUNCATE TABLE dbo.SA_Module;
INSERT INTO dbo.SA_Module(ModuleID,ModuleName,ParentID,Link,OrderNo,IsDisplay,IsFirstMenu,Flag,Create_Time) 
VALUES (1, '项目基础信息',0, '', 1, 'Y', 'Y', NULL,GETDATE());
INSERT INTO dbo.SA_Module(ModuleID,ModuleName,ParentID,Link,OrderNo,IsDisplay,IsFirstMenu,Flag,Create_Time) 
VALUES (9, '系统设置',0, '', 9, 'Y', 'Y', NULL,GETDATE());
INSERT INTO dbo.SA_Module(ModuleID,ModuleName,ParentID,Link,OrderNo,IsDisplay,IsFirstMenu,Flag,Create_Time) 
VALUES (10, '日志信息',0, '', 10, 'Y', 'Y', NULL,GETDATE());
--项目基础信息1
INSERT INTO dbo.SA_Module(ModuleID,ModuleName,ParentID,Link,OrderNo,IsDisplay,IsFirstMenu,Flag,Create_Time) 
VALUES (101, '角色管理', 1, '/Project/RoleIndex', 1, 'Y', 'N', NULL,GETDATE());
INSERT INTO dbo.SA_Module(ModuleID,ModuleName,ParentID,Link,OrderNo,IsDisplay,IsFirstMenu,Flag,Create_Time) 
VALUES (102, '账号管理', 1, '/Project/UserIndex', 2, 'Y', 'N', NULL,GETDATE());
INSERT INTO dbo.SA_Module(ModuleID,ModuleName,ParentID,Link,OrderNo,IsDisplay,IsFirstMenu,Flag,Create_Time) 
VALUES (103, '部门管理', 1, '/Project/DepartmentIndex', 3, 'Y', 'N', NULL,GETDATE());
--系统设置9
INSERT INTO dbo.SA_Module(ModuleID,ModuleName,ParentID,Link,OrderNo,IsDisplay,IsFirstMenu,Flag,Create_Time) 
VALUES (901, '系统参数设置', 9, '/SystemManage/SystemConfigIndex', 1, 'Y', 'N', NULL,GETDATE());
--日志管理10
INSERT INTO dbo.SA_Module(ModuleID,ModuleName,ParentID,Link,OrderNo,IsDisplay,IsFirstMenu,Flag,Create_Time) 
VALUES (1001, '用户登录日志', 10, '/LogManage/LogLoginIndex', 1, 'Y', 'N', NULL,GETDATE());
INSERT INTO dbo.SA_Module(ModuleID,ModuleName,ParentID,Link,OrderNo,IsDisplay,IsFirstMenu,Flag,Create_Time) 
VALUES (1002, '数据操作日志', 10, '/LogManage/LogOperationIndex', 2, 'Y', 'N', NULL,GETDATE());
GO