SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL';

-- -----------------------------------------------------
-- Table `log_base`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `log_base` ;

CREATE  TABLE IF NOT EXISTS `log_base` (
  `LOG_IDF` INT(11) NOT NULL AUTO_INCREMENT ,
  `LOG_LOGIN` VARCHAR(30) NOT NULL ,
  `LOG_DATA` DATETIME NOT NULL ,
  PRIMARY KEY (`LOG_IDF`) )
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `log_erro`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `log_erro` ;

CREATE  TABLE IF NOT EXISTS `log_erro` (
  `LOG_IDF` INT(11) NOT NULL ,
  `LOE_NOME_ENTIDADE` VARCHAR(150) NOT NULL ,
  `LOE_NOME_METODO` VARCHAR(150) NOT NULL ,
  `LOE_NOME_ERRO` VARCHAR(150) NOT NULL ,
  `LOE_DESCRICAO_ERRO` TEXT NOT NULL ,
  `LOG_IDF_PAI` INT(11) NULL DEFAULT NULL ,
  PRIMARY KEY (`LOG_IDF`) ,
  INDEX `fk_log_erro_log_base1` (`LOG_IDF` ASC) ,
  CONSTRAINT `fk_log_erro_log_base1`
    FOREIGN KEY (`LOG_IDF` )
    REFERENCES `log_base` (`LOG_IDF` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `log_trace`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `log_trace` ;

CREATE  TABLE IF NOT EXISTS `log_trace` (
  `LOG_IDF` INT(11) NOT NULL ,
  `LOC_OBSERVACAO` TEXT NOT NULL ,
  PRIMARY KEY (`LOG_IDF`) ,
  INDEX `fk_log_trace_log_base` (`LOG_IDF` ASC) ,
  CONSTRAINT `fk_log_trace_log_base`
    FOREIGN KEY (`LOG_IDF` )
    REFERENCES `log_base` (`LOG_IDF` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `tipo_operacao`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `tipo_operacao` ;

CREATE  TABLE IF NOT EXISTS `tipo_operacao` (
  `TIO_IDF` TINYINT NOT NULL ,
  `TIO_NOME` VARCHAR(15) NOT NULL ,
  PRIMARY KEY (`TIO_IDF`) )
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `log_transacao`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `log_transacao` ;

CREATE  TABLE IF NOT EXISTS `log_transacao` (
  `LOG_IDF` INT(11) NOT NULL ,
  `TIO_IDF` TINYINT(4) NOT NULL ,
  `LON_NOME_ENTIDADE` VARCHAR(50) NOT NULL ,
  `LON_VERSAO_ANTERIOR` VARCHAR(4000) NULL DEFAULT NULL ,
  `LON_OBSERVACAO` TEXT NULL DEFAULT NULL ,
  `LON_DATA_VALIDADE` DATETIME NULL DEFAULT NULL ,
  `LON_IDF_ENTIDADE` INT(11) NULL DEFAULT NULL ,
  PRIMARY KEY (`LOG_IDF`) ,
  INDEX `fk_log_transacao_log_base1` (`LOG_IDF` ASC) ,
  INDEX `fk_log_transacao_tipo_operacao1` (`TIO_IDF` ASC) ,
  CONSTRAINT `fk_log_transacao_log_base1`
    FOREIGN KEY (`LOG_IDF` )
    REFERENCES `log_base` (`LOG_IDF` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_log_transacao_tipo_operacao1`
    FOREIGN KEY (`TIO_IDF` )
    REFERENCES `tipo_operacao` (`TIO_IDF` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;



SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;

INSERT INTO TIPO_OPERACAO
VALUES	(1,'Cadastro'),
		(2,'Alteracao'),
		(3,'Exclusao'),
		(4,'Visualizacao');