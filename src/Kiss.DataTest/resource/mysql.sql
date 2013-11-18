CREATE TABLE `ttable` (
  `pk` bigint(20) NOT NULL AUTO_INCREMENT,
  `cbool` tinyint(1) DEFAULT NULL,
  `cint` int(11) DEFAULT NULL,
  `cfloat` float DEFAULT NULL,
  `cnumeric` decimal(10,2) DEFAULT NULL,
  `cstring` varchar(100) DEFAULT NULL,
  `cdate` date DEFAULT NULL,
  `cdatetime` datetime DEFAULT NULL,
  `cguid` varchar(36) DEFAULT NULL,
  `cbytes` varbinary(1000) DEFAULT NULL,
  PRIMARY KEY (`pk`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;