1�����ã�������Ӧ�ó���������ļ��У�app.config����web.config��
����������Ӵ�������Ϣ��

<configuration>

  <configSections>
      <section  name="ConnectionConfiguration"  type="Juppiphis.Data.ConnectionConfig, Juppiphis.Data"  />
  </configSections>


  <ConnectionConfiguration>
    
    <!--  ����SQL Server���ݿ�������name����Ϊ�û�����ġ����ݿ����Ӵ���������
            providerName����Ϊ���ݿ����Ӵ����ͣ�
            xml�ڵ����ı�Ϊ���Ӵ�
      -->
    <add name="IFone" providerName="System.Data.SqlClient"  >
    User Id=sa;Password=;Data Source=codeserver; Initial Catalog=IFone;
    </add>
    
    <!--  ����Oracle���ݿ����� -->
    <add name="ITKUser" providerName="Oracle.DataAccess.Client" >
      Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=CODESERVER)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=ITKUser)));User Id=test;Password=test;
    </add>
    
    <!--  ����Sybase���ݿ����� -->
    <add name="Manager" providerName="Sybase.Data.AseClient"  >
    User Id=sa;Password=;Data Source=codeserver, 2048; Initial Catalog=IFone;
    </add>
    
  </ConnectionConfiguration>
</configuration>

2���ڳ����г�ʼ�����ڳ����ʼ���ĺ�����������������䣬ע�⣬ȫ��ֻ���ʼ��һ�Σ�

		����ȫ�ֳ�ʼ������()
		{
            Juppiphis.Data.ConnectionConfig.OnInitConnectionType += OnInitConnectionType;
            Juppiphis.Data.ConnectionConfig.Configure();
        }
            
        private void OnInitConnectionType(string name, string providerName)
        {
            if (providerName == "Oracle.DataAccess.Client")
                Juppiphis.Data.ConnectionConfig.InitConnection<OracleConnection, OracleCommand, OracleParameter, OracleCommandBuilder, OracleDataAdapter>(name);
            else if (providerName == "Sybase.Data.AseClient")
                Juppiphis.Data.ConnectionConfig.InitConnection<AseConnection, AseCommand, AseParameter, AseCommandBuilder, AseDataAdapter>(name);
            else if (providerName == "System.Data.SqlClient")
                Juppiphis.Data.ConnectionConfig.InitConnection<SqlConnection, SqlCommand, SqlParameter, SqlCommandBuilder, SqlDataAdapter>(name);
            else
                return;
        }
        
3��ʹ������

DataTable table = new DataTable();

\\���к����ڶ���ֵ"IFone"�������ļ������趨�����Ӵ�������Ҳ����name="IFone"���ԣ�
Juppiphis.Data.SqlHelper.ExecuteFillTable(table, "IFone" , CommandType.Text, "Select * From Table1");
