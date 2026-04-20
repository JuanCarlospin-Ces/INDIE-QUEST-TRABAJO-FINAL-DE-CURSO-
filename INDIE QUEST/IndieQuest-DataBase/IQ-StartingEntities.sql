-- 1. Inserción de Usuarios (Basado en InMemoryUserRepository.cs)
INSERT INTO "User" (userName, email, password, ProfilePicture, userBio, availableForWork)
VALUES 
('john_doe', 'john.doe@example.com', 'password123', 'IndieQuest-LocalData/user/1/profile_john_doe.jpg', 'Software developer with a passion for open-source projects.', TRUE),
('jane_smith', 'jane.smith@example.com', 'securepassword', 'IndieQuest-LocalData/user/2/profile_jane_smith.jpg', 'Graphic designer specializing in digital art and branding.', FALSE),
('alice_wonder', 'alice.wonder@example.com', 'alicepassword', 'IndieQuest-LocalData/user/3/profile_alice_wonder.jpg', 'Content creator and social media manager with a love for storytelling.', TRUE);

-- 2. Inserción de Publicaciones (Basado en InMemoryPostRepository.cs)
INSERT INTO Post (postTitle, mediaContent, Description)
VALUES 
('First Post', 'IndieQuest-LocalData/postdata/1/media_post1.jpg', 'This is the first post.'),
('Second Post', 'IndieQuest-LocalData/postdata/2/media_post2.jpg', 'This is the second post.'),
('Third Post', 'IndieQuest-LocalData/postdata/3/media_post3.jpg', 'This is the third post.');

-- 3. Inserción de Etiquetas (Ejemplos basados en el modelo de dominio)
INSERT INTO Tag (tagName)
VALUES 
('Software'),
('Design'),
('Content');

-- 4. Relación Makes_MadeBy (Asocia Usuarios con sus Publicaciones)
-- John Doe (id 1) -> First Post (id 1)
-- Jane Smith (id 2) -> Second Post (id 2)
-- Alice Wonder (id 3) -> Third Post (id 3)
INSERT INTO Makes_MadeBy (idUser, idPost)
VALUES 
(1, 1),
(2, 2),
(3, 3);

-- 5. Relación Has_Tag (Asocia Publicaciones con Etiquetas)
INSERT INTO Has_Tag (idPost, idTag)
VALUES 
(1, 1), -- First Post tiene etiqueta Software
(2, 2), -- Second Post tiene etiqueta Design
(3, 3); -- Third Post tiene etiqueta Content