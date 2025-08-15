USE BM_SystemDB;
GO

/*��Ŀ������Ϣ*/
--��ɫ��Ϣ��
IF OBJECT_ID ('dbo.SA_Role') IS NOT NULL
	DROP TABLE dbo.SA_Role
GO
CREATE TABLE dbo.SA_Role
(
	RoleID	INT IDENTITY(1,1)	NOT NULL, --����ID
	RoleGUID	 VARCHAR(36) NOT NULL, --��ɫGUID
	RoleName	 NVARCHAR(100) NOT NULL, --��ɫ����
	RoleType     VARCHAR(20) NOT NULL, --��ɫ���ͣ�sa-��������Ա��other-����
	Notes	 NVARCHAR(200) NULL, --��ע
	Create_Time	 DATETIME NOT NULL, --����ʱ��
	CONSTRAINT PK_SA_Role PRIMARY KEY (RoleID)
)
GO
INSERT INTO dbo.SA_Role(RoleGUID,RoleName,RoleType,Notes,Create_Time) VALUES (NEWID(), '��������Ա', 'sa', 'ϵͳĬ������˺ţ�ӵ������Ȩ��',GETDATE());
GO
--����Ψһ��
ALTER TABLE dbo.SA_Role ADD CONSTRAINT UQ_SA_Role UNIQUE (RoleName);
GO
-- ��������
-- ����1����ɫGUID��ѯ����Ƶ�����ѯ��
CREATE UNIQUE NONCLUSTERED INDEX IX_RoleGUID ON dbo.SA_Role (RoleGUID) INCLUDE (RoleName, RoleType) ;
-- ����2����ɫ���Ͳ�ѯ����Ϲ���������
CREATE NONCLUSTERED INDEX IX_RoleType ON dbo.SA_Role (RoleType) INCLUDE (RoleName, RoleGUID, Create_Time);
GO

--ϵͳ�˺���Ϣ��
IF OBJECT_ID ('dbo.SA_User') IS NOT NULL
	DROP TABLE dbo.SA_User
GO
CREATE TABLE dbo.SA_User
(
	UserID	INT IDENTITY(1,1)	NOT NULL, --����ID
	UserGUID      VARCHAR(36) NOT NULL, --�û�GUID
	UserName      NVARCHAR(100) NOT NULL, --�û���
	UserPassWord  VARCHAR(36) NOT NULL, --�û����루MD5���ܣ�
	UserStatus    INT NOT NULL, --״̬��-1-���ã�0-����
	UserSex       CHAR(1) NULL, --�Ա�M-�У�F-Ů
	UserRoleID  INT NOT NULL, --��ɫID
	Phone         VARCHAR(15) NULL, --��ϵ�绰
	Email         VARCHAR(100) NULL, --�����˺�
	Create_Time	  DATETIME NOT NULL, --����ʱ��
	CONSTRAINT PK_SA_User PRIMARY KEY (UserID)
)
GO
--����Ψһ��
ALTER TABLE dbo.SA_User ADD CONSTRAINT UQ_SA_User UNIQUE (UserName);
GO
-- ��������
-- ����1���û���¼��֤����Ƶ��ѯ������
CREATE UNIQUE NONCLUSTERED INDEX IX_User_Login ON dbo.SA_User (UserName) INCLUDE (UserPassWord, UserStatus);
-- ����2����ɫ�û���ѯ����Ƶ��ϲ�ѯ��
CREATE NONCLUSTERED INDEX IX_Role_User ON dbo.SA_User (UserRoleID, UserStatus) INCLUDE (UserID, UserName, UserGUID);
-- ����3���û�״̬��ѯ����Ƶ����������
CREATE NONCLUSTERED INDEX IX_User_Status ON dbo.SA_User (UserStatus) INCLUDE (UserName, UserRoleID);
GO
INSERT INTO dbo.SA_User(UserGUID,UserName,UserPassWord,UserSex,UserRoleID,UserStatus,Phone,Email,Create_Time) VALUES (NEWID(), 'sa', 'E10ADC3949BA59ABBE56E057F20F883E', '', 1,0,NULL,NULL,GETDATE());
GO

--�˵���Ϣ��
IF OBJECT_ID ('dbo.SA_Module') IS NOT NULL
	DROP TABLE dbo.SA_Module
GO
CREATE TABLE dbo.SA_Module
(
	ModuleID	 INT NOT NULL, --�˵�ID
    ModuleName	 NVARCHAR(100) NOT NULL, --�˵�����
	ParentID	 INT NOT NULL, --�ϼ��˵�ID
	Link	     VARCHAR(100) NULL, --���ӵ�ַ
	OrderNo	     INT NOT NULL, --����
	IsDisplay	 CHAR(1) NOT NULL, --�Ƿ���ʾ��Y-��ʾ��N-����ʾ
	IsFirstMenu	 CHAR(1) NOT NULL, --�Ƿ�һ���˵���Y-�ǣ�N-����
	Flag	     INT NULL, --��ʶ
	Create_Time	 DATETIME NOT NULL, --����ʱ��
	CONSTRAINT PK_SA_Module PRIMARY KEY (ModuleID)
)
GO
--����Ψһ��
ALTER TABLE dbo.SA_Module ADD CONSTRAINT UQ_SA_Module UNIQUE (ModuleName);
GO
-- ��������
CREATE INDEX IX_SA_Module ON dbo.SA_Module (ModuleID,ModuleName,ParentID);
GO

--��ɫ�˵�Ȩ�ޱ�
IF OBJECT_ID ('dbo.SA_RolePermission') IS NOT NULL
	DROP TABLE dbo.SA_RolePermission
GO
CREATE TABLE dbo.SA_RolePermission
(
	RoleGUID	 VARCHAR(36) NOT NULL, --��ɫGUID
	ModuleID	INT NOT NULL, --�˵�ID
	CONSTRAINT PK_SA_RolePermission PRIMARY KEY (RoleGUID,ModuleID)
)
GO
-- ��������
CREATE INDEX IX_SA_RolePermission ON dbo.SA_RolePermission (RoleGUID);
GO

--������Ϣ��
IF OBJECT_ID ('dbo.SA_Department') IS NOT NULL
	DROP TABLE dbo.SA_Department
GO
CREATE TABLE dbo.SA_Department
(
	DepartmentID	INT IDENTITY(1,1)	NOT NULL, --����ID
	DepartmentGUID      VARCHAR(36) NOT NULL, --����GUID
	DepartmentName      NVARCHAR(100) NOT NULL, --��������
	Notes      NVARCHAR(200) NULL, --��ע
	Create_Time	  DATETIME NOT NULL, --����ʱ��
	CONSTRAINT PK_SA_Department PRIMARY KEY (DepartmentID)
)
GO
--����Ψһ��
ALTER TABLE dbo.SA_Department ADD CONSTRAINT UQ_SA_Department UNIQUE (DepartmentName);
GO
-- ��������
-- ����1��GUID��ѯ�Ż�����Ƶ�����ѯ��
CREATE UNIQUE NONCLUSTERED INDEX IX_DepartmentGUID ON dbo.SA_Department (DepartmentGUID) INCLUDE (DepartmentName);
GO

/*ϵͳ����*/
--ϵͳ������
IF OBJECT_ID ('dbo.SA_Parameter') IS NOT NULL
	DROP TABLE dbo.SA_Parameter
GO
CREATE TABLE dbo.SA_Parameter
(
	PID		 	    INT IDENTITY(1,1) NOT NULL,	--����ID
	ParameterName	VARCHAR(50) NOT NULL, --����
	ParameterValue	NVARCHAR(100) NOT NULL, --��ֵ
	IsDisplay	 CHAR(1) NOT NULL, --�Ƿ���ʾ��Y-��ʾ��N-����ʾ
	Notes	NVARCHAR(200) NOT NULL, --��ע
	Create_Time	 DATETIME NOT NULL, --����ʱ��
	CONSTRAINT PK_SA_Parameter PRIMARY KEY (PID)
)
GO
--����Ψһ��
ALTER TABLE dbo.SA_Parameter ADD CONSTRAINT UQ_SA_Parameter UNIQUE (ParameterName);
GO
-- ��������
CREATE INDEX IX_SA_Parameter ON dbo.SA_Parameter (ParameterName);
GO
INSERT INTO dbo.SA_Parameter(ParameterName,ParameterValue,Notes,IsDisplay,Create_Time) VALUES ('project_name', '��̨����ϵͳ','ϵͳ����', 'Y',GETDATE());
INSERT INTO dbo.SA_Parameter(ParameterName,ParameterValue,Notes,IsDisplay,Create_Time) VALUES ('company_name', 'XXXX�Ƽ����޹�˾','��˾����', 'Y',GETDATE());
GO

/*��־��Ϣ*/
--�û���¼��־��
IF OBJECT_ID ('dbo.Log_Login') IS NOT NULL
    DROP TABLE dbo.Log_Login
GO
CREATE TABLE dbo.Log_Login
(
    LogID           INT IDENTITY(1,1) NOT NULL, --��־ID������
    UserID        INT NOT NULL,     --�û�ID
	UserName        NVARCHAR(100) NOT NULL, --�û���
	UserRoleID    INT NOT NULL, --�û���ɫID
	UserRoleName	NVARCHAR(100) NOT NULL, --�û���ɫ����
    Operation_Time  DATETIME NOT NULL,         --��¼ʱ��
    Operation_IP    VARCHAR(50) NOT NULL,     --��¼IP
    Operation_Note  NVARCHAR(200) NULL,        --��ע
    CONSTRAINT PK_Log_Login PRIMARY KEY (LogID)
)
GO
-- ��������
-- 1. ʱ�䷶Χ��ѯ������95%����־��ѯ������
CREATE INDEX IX_Log_Login_OperationTime ON dbo.Log_Login (Operation_Time DESC) 
INCLUDE (UserID, UserName, UserRoleID, Operation_IP);
-- 2. �û���Ϊ��������
CREATE INDEX IX_Log_Login_User ON dbo.Log_Login (UserID, Operation_Time DESC) 
INCLUDE (UserRoleID, Operation_IP);
-- 3. ��ɫ��Ϊ��������
CREATE INDEX IX_Log_Login_Role ON dbo.Log_Login (UserRoleID, Operation_Time DESC) 
INCLUDE (UserID, Operation_IP);
GO

--���ݲ�����־��
IF OBJECT_ID ('dbo.Log_Operation') IS NOT NULL
	DROP TABLE dbo.Log_Operation
GO
CREATE TABLE dbo.Log_Operation
(
    LogID			INT IDENTITY(1,1) NOT NULL,	--��־ID������
	Object_Name		VARCHAR(50) NOT NULL, --��������
	UserID	    INT NOT NULL, --�����û�ID
	UserName        NVARCHAR(100) NOT NULL, --�����û���
	UserRoleID    INT NOT NULL, --�����û���ɫID
	UserRoleName	NVARCHAR(100) NOT NULL, --�����û���ɫ����
	Operation_Time	DATETIME NOT NULL, --����ʱ��
	Operation_Type	VARCHAR(50) NOT NULL, --�������ͣ����磺Add��Update��Delete��Import��
	Operation_IP	VARCHAR(50) NOT NULL, --����IP
	Operation_Note	NVARCHAR(200) NULL, --��ע
	CONSTRAINT PK_Log_Operation PRIMARY KEY (LogID)
)
GO
-- ����1��ʱ�䷶Χ��ѯ����Ƶ������
CREATE INDEX IX_Operation_Time ON dbo.Log_Operation (Operation_Time);
-- ����2���û���Ϊ׷�٣���Ƶ��ϲ�ѯ��
CREATE INDEX IX_User_Action ON dbo.Log_Operation (UserID, Operation_Type) INCLUDE (Operation_Time, Object_Name);
-- ����3�����������ƣ��������ѯ��
CREATE INDEX IX_Object_Operation ON dbo.Log_Operation (Object_Name, Operation_Type) INCLUDE (Operation_Time, UserID);
-- ����4����ɫ��������������ɫͳ�ƣ�
CREATE INDEX IX_Role_Operation ON dbo.Log_Operation (UserRoleID, Operation_Time) INCLUDE (Operation_Type);
GO

--�����޸ĺۼ���־��
IF OBJECT_ID ('dbo.Log_Trace') IS NOT NULL
	DROP TABLE dbo.Log_Trace
GO
CREATE TABLE dbo.Log_Trace
(
	LogID			INT IDENTITY(1,1) NOT NULL,	--��־ID������
	Object_Type		NVARCHAR(50) NOT NULL, --�����������ͣ����磺�û���user��
	Object_GUID		VARCHAR(36) NOT NULL, --��������ID
	Object_Name		NVARCHAR(100) NOT NULL, --����������
	Object_Field    VARCHAR(50) NULL, --�����������ԣ����磺�û����ƣ�UserName��
	Old_Value		NVARCHAR(300) NULL, --��ֵ
	New_Value		NVARCHAR(300) NULL, --��ֵ
	UserID	    INT NOT NULL, --�����û�ID
	UserName        NVARCHAR(100) NOT NULL, --�����û���
	UserRoleID    INT NOT NULL, --�����û���ɫID
	UserRoleName	NVARCHAR(100) NOT NULL, --�����û���ɫ����
	Operation_Time	DATETIME NOT NULL, --����ʱ��
	Operation_Type	VARCHAR(50) NOT NULL, --�������ͣ����磺Add��Update��Delete��Import��
	Operation_IP	VARCHAR(50) NOT NULL, --����IP
	Operation_Note	NVARCHAR(200) NULL, --��ע
	CONSTRAINT PK_Log_Trace PRIMARY KEY (LogID)
)
GO
-- ����1������׷�ٲ�ѯ����Ƶ��ѯ������
CREATE INDEX IX_Object_Trace ON dbo.Log_Trace (Object_Type,Object_GUID,Operation_Time DESC) INCLUDE (UserID, UserName, Operation_Type);
-- ����2����Ʒ�����ʱ�䷶Χ+�������ͣ�
CREATE INDEX IX_Audit_Analysis ON dbo.Log_Trace (Operation_Type,Operation_Time DESC) INCLUDE (Object_Name, UserRoleName, Operation_IP);
-- ����3���û���Ϊ׷�٣�������
CREATE INDEX IX_User_Behavior ON dbo.Log_Trace (UserID,Operation_Time DESC) INCLUDE (Object_Type, Operation_Type);
-- ����4����ɫ������ƣ�������
CREATE INDEX IX_Role_Audit ON dbo.Log_Trace (UserRoleID,Operation_Time DESC) INCLUDE (Operation_Type, Object_Type);
GO




/*��ͼ*/
--��ȡ�˺���Ϣ
IF OBJECT_ID ('dbo.SA_User_v') IS NOT NULL
	DROP VIEW dbo.SA_User_v
GO
CREATE VIEW [dbo].[SA_User_v] AS 
SELECT a.*,b.RoleName
FROM dbo.SA_User a
LEFT JOIN dbo.SA_Role b ON a.UserRoleID=b.RoleID
GO

--��ȡ�û���¼��־��Ϣ
IF OBJECT_ID ('dbo.Log_Login_v') IS NOT NULL
	DROP VIEW dbo.Log_Login_v
GO
CREATE VIEW [dbo].[Log_Login_v] AS 
SELECT a.LogID,a.UserID,a.UserRoleID,a.Operation_Time,a.Operation_IP,a.Operation_Note,
	ISNULL(b.UserName,a.UserName) AS UserName,ISNULL(b.RoleName,a.UserRoleName) AS UserRoleName
FROM dbo.Log_Login a
LEFT JOIN dbo.SA_User_v b ON a.UserID=b.UserID
GO

--��ȡ������־��Ϣ
IF OBJECT_ID ('dbo.Log_Operation_v') IS NOT NULL
	DROP VIEW dbo.Log_Operation_v
GO
CREATE VIEW [dbo].[Log_Operation_v] AS 
SELECT a.LogID,a.Object_Name,a.UserID,a.UserRoleID,a.Operation_Time,a.Operation_Type,
	a.Operation_Note,a.Operation_IP,ISNULL(b.UserName,a.UserName) AS UserName,ISNULL(b.RoleName,a.UserRoleName) AS UserRoleName
FROM dbo.Log_Operation a
LEFT JOIN dbo.SA_User_v b ON a.UserID=b.UserID
GO

--��ȡ�����޸ĺۼ���־��Ϣ
IF OBJECT_ID ('dbo.Log_Trace_v') IS NOT NULL
	DROP VIEW dbo.Log_Trace_v
GO
CREATE VIEW [dbo].[Log_Trace_v] AS 
--��ɫ��Ϣ��־
SELECT a.LogID,a.Object_Type,a.Object_GUID,ISNULL(c.RoleName,a.Object_Name) AS Object_Name,a.Object_Field,a.Old_Value,a.New_Value,a.UserID,a.UserRoleID,a.Operation_Time,
	a.Operation_Type,a.Operation_Note,a.Operation_IP,ISNULL(b.UserName,a.UserName) AS UserName,ISNULL(b.RoleName,a.UserRoleName) AS UserRoleName
FROM dbo.Log_Trace a WITH (INDEX(IX_Object_Trace))
LEFT JOIN dbo.SA_User_v b ON a.UserID=b.UserID
LEFT JOIN dbo.SA_Role c ON a.Object_GUID=c.RoleGUID
WHERE a.Object_Type='role'
UNION
--�˺���Ϣ��־
SELECT a.LogID,a.Object_Type,a.Object_GUID,ISNULL(c.UserName,a.Object_Name) AS Object_Name,a.Object_Field,a.Old_Value,a.New_Value,a.UserID,a.UserRoleID,a.Operation_Time,
	a.Operation_Type,a.Operation_Note,a.Operation_IP,ISNULL(b.UserName,a.UserName) AS UserName,ISNULL(b.RoleName,a.UserRoleName) AS UserRoleName
FROM dbo.Log_Trace a WITH (INDEX(IX_Object_Trace))
LEFT JOIN dbo.SA_User_v b ON a.UserID=b.UserID
LEFT JOIN dbo.SA_User c ON a.Object_GUID=c.UserGUID
WHERE a.Object_Type='user'
UNION
--������Ϣ��־
SELECT a.LogID,a.Object_Type,a.Object_GUID,ISNULL(c.DepartmentName,a.Object_Name) AS Object_Name,a.Object_Field,a.Old_Value,a.New_Value,a.UserID,a.UserRoleID,a.Operation_Time,
	a.Operation_Type,a.Operation_Note,a.Operation_IP,ISNULL(b.UserName,a.UserName) AS UserName,ISNULL(b.RoleName,a.UserRoleName) AS UserRoleName
FROM dbo.Log_Trace a WITH (INDEX(IX_Object_Trace))
LEFT JOIN dbo.SA_User_v b ON a.UserID=b.UserID
LEFT JOIN dbo.SA_Department c ON a.Object_GUID=c.DepartmentGUID
WHERE a.Object_Type='department'
UNION
--����������Ϣ��־
SELECT a.LogID,a.Object_Type,a.Object_GUID,ISNULL(c.ParameterName,a.Object_Name) AS Object_Name,a.Object_Field,a.Old_Value,a.New_Value,a.UserID,a.UserRoleID,a.Operation_Time,
	a.Operation_Type,a.Operation_Note,a.Operation_IP,ISNULL(b.UserName,a.UserName) AS UserName,ISNULL(b.RoleName,a.UserRoleName) AS UserRoleName
FROM dbo.Log_Trace a WITH (INDEX(IX_Object_Trace))
LEFT JOIN dbo.SA_User_v b ON a.UserID=b.UserID
LEFT JOIN dbo.SA_Parameter c ON a.Object_GUID=c.ParameterName
WHERE a.Object_Type='parameter'
GO


--����˵�����
TRUNCATE TABLE dbo.SA_Module;
INSERT INTO dbo.SA_Module(ModuleID,ModuleName,ParentID,Link,OrderNo,IsDisplay,IsFirstMenu,Flag,Create_Time) 
VALUES (1, '��Ŀ������Ϣ',0, '', 1, 'Y', 'Y', NULL,GETDATE());
INSERT INTO dbo.SA_Module(ModuleID,ModuleName,ParentID,Link,OrderNo,IsDisplay,IsFirstMenu,Flag,Create_Time) 
VALUES (9, 'ϵͳ����',0, '', 9, 'Y', 'Y', NULL,GETDATE());
INSERT INTO dbo.SA_Module(ModuleID,ModuleName,ParentID,Link,OrderNo,IsDisplay,IsFirstMenu,Flag,Create_Time) 
VALUES (10, '��־��Ϣ',0, '', 10, 'Y', 'Y', NULL,GETDATE());
--��Ŀ������Ϣ1
INSERT INTO dbo.SA_Module(ModuleID,ModuleName,ParentID,Link,OrderNo,IsDisplay,IsFirstMenu,Flag,Create_Time) 
VALUES (101, '��ɫ����', 1, '/Project/RoleIndex', 1, 'Y', 'N', NULL,GETDATE());
INSERT INTO dbo.SA_Module(ModuleID,ModuleName,ParentID,Link,OrderNo,IsDisplay,IsFirstMenu,Flag,Create_Time) 
VALUES (102, '�˺Ź���', 1, '/Project/UserIndex', 2, 'Y', 'N', NULL,GETDATE());
INSERT INTO dbo.SA_Module(ModuleID,ModuleName,ParentID,Link,OrderNo,IsDisplay,IsFirstMenu,Flag,Create_Time) 
VALUES (103, '���Ź���', 1, '/Project/DepartmentIndex', 3, 'Y', 'N', NULL,GETDATE());
--ϵͳ����9
INSERT INTO dbo.SA_Module(ModuleID,ModuleName,ParentID,Link,OrderNo,IsDisplay,IsFirstMenu,Flag,Create_Time) 
VALUES (901, 'ϵͳ��������', 9, '/SystemManage/SystemConfigIndex', 1, 'Y', 'N', NULL,GETDATE());
--��־����10
INSERT INTO dbo.SA_Module(ModuleID,ModuleName,ParentID,Link,OrderNo,IsDisplay,IsFirstMenu,Flag,Create_Time) 
VALUES (1001, '�û���¼��־', 10, '/LogManage/LogLoginIndex', 1, 'Y', 'N', NULL,GETDATE());
INSERT INTO dbo.SA_Module(ModuleID,ModuleName,ParentID,Link,OrderNo,IsDisplay,IsFirstMenu,Flag,Create_Time) 
VALUES (1002, '���ݲ�����־', 10, '/LogManage/LogOperationIndex', 2, 'Y', 'N', NULL,GETDATE());
GO