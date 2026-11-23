-- Gestion scolaire - MySQL 8.4 
CREATE DATABASE IF NOT EXISTS gestion_scolaire;
USE gestion_scolaire;

CREATE TABLE IF NOT EXISTS filiere (
  id INT AUTO_INCREMENT PRIMARY KEY,
  code VARCHAR(10) NOT NULL UNIQUE,
  libelle VARCHAR(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS etudiant (
  id INT AUTO_INCREMENT PRIMARY KEY,
  matricule VARCHAR(15) NOT NULL UNIQUE,
  nom VARCHAR(50) NOT NULL,
  prenom VARCHAR(50) NOT NULL,
  date_naissance DATE NULL,
  email VARCHAR(100) NULL,
  filiere_id INT NOT NULL,
  date_inscription DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_etudiant_filiere FOREIGN KEY (filiere_id)
    REFERENCES filiere(id) ON UPDATE CASCADE ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS matiere (
  id INT AUTO_INCREMENT PRIMARY KEY,
  code VARCHAR(10) NOT NULL UNIQUE,
  intitule VARCHAR(100) NOT NULL,
  coefficient DECIMAL(3,1) NOT NULL DEFAULT 1.0,
  CONSTRAINT ck_matiere_coefficient CHECK (coefficient > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS note (
  id INT AUTO_INCREMENT PRIMARY KEY,
  etudiant_id INT NOT NULL,
  matiere_id INT NOT NULL,
  valeur DECIMAL(4,2) NOT NULL,
  CONSTRAINT ck_note_valeur CHECK (valeur BETWEEN 0 AND 20),
  CONSTRAINT uq_note UNIQUE (etudiant_id, matiere_id),
  CONSTRAINT fk_note_etudiant FOREIGN KEY (etudiant_id)
    REFERENCES etudiant(id) ON UPDATE CASCADE ON DELETE CASCADE,
  CONSTRAINT fk_note_matiere FOREIGN KEY (matiere_id)
    REFERENCES matiere(id) ON UPDATE CASCADE ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO filiere(code,libelle) VALUES
('GL','Génie logiciel'),('RSI','Réseaux et sécurité informatique'),
('SIG','Systèmes d’information et gestion');

INSERT IGNORE INTO matiere(code,intitule,coefficient) VALUES
('ALG','Algorithmique',3.0),('BDD','Bases de données',3.0),
('POO','Programmation orientée objet',4.0),('ANG','Anglais technique',1.0);

INSERT IGNORE INTO etudiant(matricule,nom,prenom,date_naissance,email,filiere_id) VALUES
('GL2026-001','Mbarga','Alice','2004-03-12','alice.mbarga@example.org',1),
('GL2026-002','Tchoumi','Brice','2003-11-02','brice.tchoumi@example.org',1),
('RS2026-001','Fotso','Carine','2004-07-25',NULL,2),
('SI2026-001','Ngono','David',NULL,'david.ngono@example.org',3);

INSERT IGNORE INTO note(etudiant_id,matiere_id,valeur) VALUES
(1,1,15.50),(1,2,14.00),(1,3,17.25),(1,4,12.00),
(2,1,11.00),(2,2,9.50),(2,3,13.00),(3,1,16.00),(3,2,15.00);

-- Compte dédié à l'application. Remplacer le mot de passe avant exécution.
CREATE USER IF NOT EXISTS 'app_scolaire'@'localhost'
  IDENTIFIED BY 'CHANGE_ME_Strong#2026';
GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE
  ON gestion_scolaire.* TO 'app_scolaire'@'localhost';
FLUSH PRIVILEGES;
