using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDA.FileStorage
{
    /// <summary>
    /// An implementation of the IFileStore that stores files in a database.
    /// </summary>
    public class DbFileStore : IFileStore
    {
        private string connString;

        public DbFileStore(string connectionString)
        {
            connString = connectionString;
        }

        private void RunCommand(SqlCommand cmd, Action action)
        {
            using (var conn = new SqlConnection(connString))
            {
                cmd.Connection = conn;
                conn.Open();
                action();
                conn.Close();
            }
        }

        public void Initialize()
        {
            var cmd = new SqlCommand(@"if not exists (select * from sysobjects where name='FileStore' and xtype='U')
	            create table dbo.FileStore
	            (
		            Id nvarchar(100) primary key clustered,
		            [Content] varbinary(MAX) NULL
	            )  on [PRIMARY]
	             textimage_on [PRIMARY]");

            RunCommand(cmd, () => cmd.ExecuteNonQuery());
        }

        public Stream GetStream(string fileId)
        {
            byte[] bytes = new byte[] { };

            var cmd = new SqlCommand("select Content from FileStore where Id = @id");
            cmd.Parameters.Add(new SqlParameter("@id", System.Data.SqlDbType.NVarChar));
            cmd.Parameters["@id"].Value = fileId;

            RunCommand(cmd, () =>
            {
                bytes = cmd.ExecuteScalar() as byte[];
            });

            return new MemoryStream(bytes);
        }

        public void Save(string fileId, Stream stream, bool directlyCloseStream = true)
        {
            var sql = @"if exists (select Id from FileStore where Id=@id)
                update FileStore
                set Content = @content
                where Id = @id
            else
                insert into FileStore(Id, Content)
                values(@id, @content)";

            ReadBytesAndRunSql(sql, fileId, stream, directlyCloseStream);
        }

        public void Overwrite(string fileId, Stream stream, bool directlyCloseStream = true)
        {
            var sql = "update FileStore set Content=@content where Id=@id";
            ReadBytesAndRunSql(sql, fileId, stream, directlyCloseStream);
        }

        private void ReadBytesAndRunSql(string sql, string fileId, Stream stream, bool directlyCloseStream)
        {
            // Read stream
            byte[] bytes = FileHelper.ReadBytes(stream);
            if (directlyCloseStream)
                stream.Close();

            var cmd = new SqlCommand(sql);
            cmd.Parameters.Add(new SqlParameter("@id", System.Data.SqlDbType.NVarChar));
            cmd.Parameters.Add(new SqlParameter("@content", System.Data.SqlDbType.VarBinary));
            cmd.Parameters["@id"].Value = fileId;
            cmd.Parameters["@content"].Value = bytes;

            RunCommand(cmd, () => cmd.ExecuteNonQuery());
        }

        public void Delete(string fileId)
        {
            var cmd = new SqlCommand("delete FileStore where Id=@id");
            cmd.Parameters.Add(new SqlParameter("@id", System.Data.SqlDbType.NVarChar));
            cmd.Parameters["@id"].Value = fileId;

            RunCommand(cmd, () => cmd.ExecuteNonQuery());
        }
    }
}
