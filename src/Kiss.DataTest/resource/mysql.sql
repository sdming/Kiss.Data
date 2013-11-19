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

DELIMITER //
CREATE PROCEDURE `usp_query`(p_cint int)
BEGIN         
   SELECT * FROM `ttable` WHERE `cint` = p_cint;
END//
DELIMITER ;

DELIMITER //
CREATE PROCEDURE `usp_exec`(p_cint int)
BEGIN         
   DELETE FROM `ttable` WHERE `cint` = p_cint;
END//
DELIMITER ;

DELIMITER //
CREATE PROCEDURE `usp_inout`(IN x int, INOUT y int, OUT sum int)
BEGIN         
  SET sum = x + y;
  SET y = 2 * y;
END//
DELIMITER ;