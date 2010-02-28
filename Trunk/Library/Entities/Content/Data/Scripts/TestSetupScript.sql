/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add Setup Script */
/********************/

SET IDENTITY_INSERT  {databaseOwner}{objectQualifier}ContentTypes ON
INSERT INTO {databaseOwner}{objectQualifier}ContentTypes ([ContentTypeID], [ContentType]) VALUES (1, 'Tab')
INSERT INTO {databaseOwner}{objectQualifier}ContentTypes ([ContentTypeID], [ContentType]) VALUES (2, 'Module')
INSERT INTO {databaseOwner}{objectQualifier}ContentTypes ([ContentTypeID], [ContentType]) VALUES (3, 'TestDelete')
INSERT INTO {databaseOwner}{objectQualifier}ContentTypes ([ContentTypeID], [ContentType]) VALUES (4, 'TestUpdate')
INSERT INTO {databaseOwner}{objectQualifier}ContentTypes ([ContentTypeID], [ContentType]) VALUES (5, 'TestGetByName')
SET IDENTITY_INSERT  {databaseOwner}{objectQualifier}ContentTypes OFF

SET IDENTITY_INSERT  {databaseOwner}{objectQualifier}Taxonomy_ScopeTypes ON
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_ScopeTypes ([ScopeTypeID], [ScopeType]) VALUES (1, 'Application')
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_ScopeTypes ([ScopeTypeID], [ScopeType]) VALUES (2, 'Portal')
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_ScopeTypes ([ScopeTypeID], [ScopeType]) VALUES (3, 'TestDelete')
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_ScopeTypes ([ScopeTypeID], [ScopeType]) VALUES (4, 'TestUpdate')
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_ScopeTypes ([ScopeTypeID], [ScopeType]) VALUES (5, 'TestGetByName')
SET IDENTITY_INSERT  {databaseOwner}{objectQualifier}Taxonomy_ScopeTypes OFF

SET IDENTITY_INSERT  {databaseOwner}{objectQualifier}Taxonomy_VocabularyTypes ON
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_VocabularyTypes ([VocabularyTypeID], [VocabularyType]) VALUES (1, 'Simple')
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_VocabularyTypes ([VocabularyTypeID], [VocabularyType]) VALUES (2, 'Heirarchy')
SET IDENTITY_INSERT  {databaseOwner}{objectQualifier}Taxonomy_VocabularyTypes OFF

SET IDENTITY_INSERT  {databaseOwner}{objectQualifier}Taxonomy_Vocabularies ON
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Vocabularies ([VocabularyID], [VocabularyTypeID], [Name], [Description], [ScopeTypeID]) 
			VALUES (1, 1, 'Colors', 'This is a simple list Vocabulary of Colors', 1)
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Vocabularies ([VocabularyID], [VocabularyTypeID], [Name], [Description], [ScopeTypeID]) 
			VALUES (2, 2, 'Electronics', 'This Vocabulary is for Electronic Products', 1)
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Vocabularies ([VocabularyID], [VocabularyTypeID], [Name], [Description], [ScopeTypeID], [ScopeID]) 
			VALUES (3, 2, 'TestDelete', '', 2, 1)
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Vocabularies ([VocabularyID], [VocabularyTypeID], [Name], [Description], [ScopeTypeID], [ScopeID]) 
			VALUES (4, 2, 'TestUpdate', '', 2, 1)
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Vocabularies ([VocabularyID], [VocabularyTypeID], [Name], [Description], [ScopeTypeID]) 
			VALUES (5, 2, 'Test', '', 1)
SET IDENTITY_INSERT  {databaseOwner}{objectQualifier}Taxonomy_Vocabularies OFF

SET IDENTITY_INSERT  {databaseOwner}{objectQualifier}Taxonomy_Terms ON
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Terms ([TermID], [VocabularyID], [ParentTermID], [Name], [TermLeft], [TermRight]) 
			VALUES (12, 2, NULL, 'Electronics', 1, 18)
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Terms ([TermID], [VocabularyID], [ParentTermID], [Name], [TermLeft], [TermRight]) 
			VALUES (1, 2, 12, 'Televisions', 12, 17)
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Terms ([TermID], [VocabularyID], [ParentTermID], [Name], [TermLeft], [TermRight]) 
			VALUES (2, 2, 12, 'Portable Electronics', 2, 11)
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Terms ([TermID], [VocabularyID], [ParentTermID], [Name], [TermLeft], [TermRight]) 
			VALUES (3, 2, 1, 'Tube', 15, 16)
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Terms ([TermID], [VocabularyID], [ParentTermID], [Name], [TermLeft], [TermRight]) 
			VALUES (4, 2, 1, 'LCD', 13, 14)
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Terms ([TermID], [VocabularyID], [ParentTermID], [Name], [TermLeft], [TermRight]) 
			VALUES (6, 2, 2, 'MP3 PLayers', 7, 10)
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Terms ([TermID], [VocabularyID], [ParentTermID], [Name], [TermLeft], [TermRight]) 
			VALUES (7, 2, 2, 'CD PLayers', 5, 6)
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Terms ([TermID], [VocabularyID], [ParentTermID], [Name], [TermLeft], [TermRight]) 
			VALUES (8, 2, 2, '2 Way Radios', 3, 4)
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Terms ([TermID], [VocabularyID], [ParentTermID], [Name], [TermLeft], [TermRight]) 
			VALUES (9, 2, 6, 'Flash', 8, 9)
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Terms ([TermID], [VocabularyID], [ParentTermID], [Name], [TermLeft], [TermRight]) 
			VALUES (10, 1, NULL, 'Red', 0, 0)
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Terms ([TermID], [VocabularyID], [ParentTermID], [Name], [TermLeft], [TermRight]) 
			VALUES (11, 1, NULL, 'Blue', 0, 0)
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Terms ([TermID], [VocabularyID], [ParentTermID], [Name], [TermLeft], [TermRight]) 
			VALUES (13, 1, NULL, 'Black', 0, 0)
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Terms ([TermID], [VocabularyID], [ParentTermID], [Name], [TermLeft], [TermRight]) 
			VALUES (14, 1, NULL, 'Silver', 0, 0)
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Terms ([TermID], [VocabularyID], [ParentTermID], [Name], [TermLeft], [TermRight]) 
			VALUES (15, 1, NULL, 'Yellow', 0, 0)
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Terms ([TermID], [VocabularyID], [ParentTermID], [Name], [TermLeft], [TermRight]) 
			VALUES (16, 3, NULL, 'Test Delete', 1, 2)
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Terms ([TermID], [VocabularyID], [ParentTermID], [Name], [TermLeft], [TermRight]) 
			VALUES (17, 4, NULL, 'Test Update', 1, 6)
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Terms ([TermID], [VocabularyID], [ParentTermID], [Name], [TermLeft], [TermRight]) 
			VALUES (18, 4, 17, 'Test Child 1', 2, 3)
INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Terms ([TermID], [VocabularyID], [ParentTermID], [Name], [TermLeft], [TermRight]) 
			VALUES (19, 4, 17, 'Test Child 2', 4, 5)
SET IDENTITY_INSERT  {databaseOwner}{objectQualifier}Taxonomy_Terms OFF

SET IDENTITY_INSERT  {databaseOwner}{objectQualifier}ContentItems ON
INSERT INTO {databaseOwner}{objectQualifier}ContentItems ([ContentItemID], [Content], [ContentKey], [Indexed]) 
			VALUES (1, 'Content 1', 'ContentKey 1', 1)
INSERT INTO {databaseOwner}{objectQualifier}ContentItems ([ContentItemID], [Content], [ContentKey], [Indexed]) 
			VALUES (2, 'Content 2', 'ContentKey 2', 1)
INSERT INTO {databaseOwner}{objectQualifier}ContentItems ([ContentItemID], [Content], [ContentKey], [Indexed])
			VALUES (3, 'Content 3', 'ContentKey 3', 0)
INSERT INTO {databaseOwner}{objectQualifier}ContentItems ([ContentItemID], [Content], [ContentKey], [Indexed])
			VALUES (4, 'Content 4', 'ContentKey 4', 0)
INSERT INTO {databaseOwner}{objectQualifier}ContentItems ([ContentItemID], [Content], [ContentKey], [Indexed])
			VALUES (5, 'Content 5', 'ContentKey 5', 0)
SET IDENTITY_INSERT  {databaseOwner}{objectQualifier}ContentItems OFF

SET IDENTITY_INSERT  {databaseOwner}{objectQualifier}ContentItems_Tags ON
INSERT INTO {databaseOwner}{objectQualifier}ContentItems_Tags ([ContentItemTagID], [ContentItemID], [TermID]) 
			VALUES (1, 1, 6)
INSERT INTO {databaseOwner}{objectQualifier}ContentItems_Tags ([ContentItemTagID], [ContentItemID], [TermID]) 
			VALUES (2, 1, 11)
INSERT INTO {databaseOwner}{objectQualifier}ContentItems_Tags ([ContentItemTagID], [ContentItemID], [TermID])
			VALUES (3, 2, 4)
SET IDENTITY_INSERT  {databaseOwner}{objectQualifier}ContentItems_Tags OFF

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
