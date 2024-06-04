CREATE TABLE __dane_osobowe (
    id INT IDENTITY(1,1) PRIMARY KEY,
    adres_email VARCHAR(255) NOT NULL,
    haslo_szyfrowane VARBINARY(255) NOT NULL,
    imie VARCHAR(50) NOT NULL,
    nazwisko VARCHAR(50) NOT NULL,
    plec VARCHAR(10),
    wiek INT,
    waga FLOAT
);

-- Tworzenie tabeli leki
CREATE TABLE __leki (
    id_leku INT IDENTITY(1,1) PRIMARY KEY,
    nazwa_leku VARCHAR(100) NOT NULL,
);



-- Tworzenie tabeli przypomnienia
CREATE TABLE __przypomnienia (
    id INT IDENTITY(1,1) PRIMARY KEY,
    id_leku INT FOREIGN KEY REFERENCES __leki(id_leku),
    data_przypomnienia DATE,
    godzina_przypomnienia TIME
);

-- Tworzenie tabeli lista_lekow
CREATE TABLE __lista_lekow (
    id INT IDENTITY(1,1) PRIMARY KEY,
    id_leku INT FOREIGN KEY REFERENCES __leki(id_leku),
    dawka VARCHAR(50),
    plec VARCHAR(10),
    wiek INT,
    waga FLOAT
);