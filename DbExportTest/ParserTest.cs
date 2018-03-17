﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DbExport.Providers.SQLite.SqlParser;
using System.Collections.Generic;

namespace DbExportTest
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod]
        public void TestCreateTable()
        {
            var input =
@"CREATE TABLE [app_traitement_marche] (
	[id_traitement] integer NOT NULL UNIQUE,
	[code_exercice] char(2) NOT NULL,
	[num_session] tinyint NOT NULL,
	[code_admin_benef] char(10) NOT NULL,
	[num_marche] smallint NOT NULL,
	[code_type_traitement] tinyint NOT NULL,
	[date_traitement] datetime NOT NULL,
	[previsionnel] tinyint NOT NULL DEFAULT 0,
	PRIMARY KEY ([id_traitement]),
	CONSTRAINT [UK_app_traitement_marche] UNIQUE ([code_exercice], [num_session], [code_admin_benef], [num_marche], [code_type_traitement]),
	CONSTRAINT [CK_previsionnel_bit] CHECK (previsionnel = 0 OR previsionnel = 1),
	CONSTRAINT [FK_app_traitement_marche_app_marche] FOREIGN KEY ([code_exercice], [code_admin_benef], [num_marche]) REFERENCES [app_marche] ([code_exercice], [code_admin_benef], [num_marche]) ON UPDATE CASCADE ON DELETE CASCADE,
	CONSTRAINT [FK_app_traitement_marche_app_session] FOREIGN KEY ([code_exercice], [num_session]) REFERENCES [app_session] ([code_exercice], [num_session]) ON UPDATE CASCADE ON DELETE CASCADE,
	CONSTRAINT [FK_app_traitement_marche_app_type_traitement] FOREIGN KEY ([code_exercice], [code_type_traitement]) REFERENCES [app_type_traitement] ([code_exercice], [code_type_traitement]) ON UPDATE CASCADE ON DELETE CASCADE
)";
            var parser = new Parser(new Scanner(input));
            var node = parser.CreateTable();

            Assert.AreEqual(node.Kind, AstNodeKind.CREATE_TBL);
            Assert.AreEqual(node.ChildNodes.Count, 7);
            Assert.AreEqual(node.Data, "app_traitement_marche");
        }
    }
}
