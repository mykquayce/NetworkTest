CREATE TABLE `networktest`.`Results` (
  `DateTime` DATETIME NOT NULL,
  `Count` INT NOT NULL,
  `FailedCount` INT NOT NULL,
  `PacketLossPercentage` DOUBLE NOT NULL,
  `AverageRoundtripTime` DOUBLE NOT NULL,
  `AverageJitter` DOUBLE NOT NULL,
  PRIMARY KEY (`DateTime`),
  UNIQUE INDEX `DateTime_UNIQUE` (`DateTime` ASC) VISIBLE);
