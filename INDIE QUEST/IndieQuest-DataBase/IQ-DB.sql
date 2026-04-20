-- Reset de la base de datos

DROP TABLE IF EXISTS Has_Tag CASCADE;
DROP TABLE IF EXISTS Makes_MadeBy CASCADE;
DROP TABLE IF EXISTS Tag CASCADE;
DROP TABLE IF EXISTS Post CASCADE;
DROP TABLE IF EXISTS "User" CASCADE;

-- 1. Creación de la tabla User
CREATE TABLE "User" (
    idUser SERIAL PRIMARY KEY,
    userName VARCHAR(255) NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    ProfilePicture VARCHAR(255),
    userBio TEXT,
    availableForWork BOOLEAN DEFAULT FALSE,
    dateOfRegistration DATE DEFAULT CURRENT_DATE
);

-- 2. Creación de la tabla Post
CREATE TABLE Post (
    idPost SERIAL PRIMARY KEY,
    postTitle VARCHAR(255) NOT NULL,
    mediaContent VARCHAR(255),
    Description TEXT,
    CreationDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 3. Creación de la tabla Tag
CREATE TABLE Tag (
    idTag SERIAL PRIMARY KEY,
    tagName VARCHAR(100) UNIQUE NOT NULL
);

-- 4. Creación de la tabla de relación 'Makes_MadeBy' (1:N)
-- Representa la relación entre User y Post.
CREATE TABLE Makes_MadeBy (
    idUser INT REFERENCES "User"(idUser) ON DELETE CASCADE,
    idPost INT REFERENCES Post(idPost) ON DELETE CASCADE,
    PRIMARY KEY (idUser, idPost)
);

-- 5. Creación de la tabla de relación 'Has_Tag' (N:M)
-- Representa la relación entre Post y Tag.
CREATE TABLE Has_Tag (
    idPost INT REFERENCES Post(idPost) ON DELETE CASCADE,
    idTag INT REFERENCES Tag(idTag) ON DELETE CASCADE,
    PRIMARY KEY (idPost, idTag)
);