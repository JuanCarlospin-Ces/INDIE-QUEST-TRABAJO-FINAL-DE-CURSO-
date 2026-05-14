-- 1. Inserción de Usuarios (15 usuarios para probar paginación con pageSize=10)
INSERT INTO "User" (userName, email, password, ProfilePicture, userBio, availableForWork)
VALUES 
('john_doe',     'john.doe@example.com',     'password123',    'IndieQuest-LocalData/user/1/profile.jpg',  'Software developer with a passion for open-source projects.',           TRUE),
('jane_smith',   'jane.smith@example.com',   'securepassword', 'IndieQuest-LocalData/user/2/profile.jpg',  'Graphic designer specializing in digital art and branding.',            FALSE),
('alice_wonder', 'alice.wonder@example.com', 'alicepassword',  'IndieQuest-LocalData/user/3/profile.jpg',  'Content creator and social media manager with a love for storytelling.', TRUE),
('carlos_dev',   'carlos.dev@example.com',   'carlos123',      'IndieQuest-LocalData/user/4/profile.jpg',  'Indie game developer focused on pixel art and retro games.',            TRUE),
('mia_pixel',    'mia.pixel@example.com',    'mia456',         'IndieQuest-LocalData/user/5/profile.jpg',  'Digital artist and game jam enthusiast.',                               FALSE),
('luke_coder',   'luke.coder@example.com',   'luke789',        'IndieQuest-LocalData/user/6/profile.jpg',  'Full-stack developer exploring game mechanics and procedural generation.',TRUE),
('sara_art',     'sara.art@example.com',     'sara321',        'IndieQuest-LocalData/user/7/profile.jpg',  'Concept artist creating environments for indie RPGs.',                  FALSE),
('tom_design',   'tom.design@example.com',   'tom654',         'IndieQuest-LocalData/user/8/profile.jpg',  'UX/UI designer bringing usability to indie game interfaces.',           TRUE),
('nina_dev',     'nina.dev@example.com',     'nina987',        'IndieQuest-LocalData/user/9/profile.jpg',  'Backend developer building game server infrastructure.',                FALSE),
('max_indie',    'max.indie@example.com',    'max111',         'IndieQuest-LocalData/user/10/profile.jpg', 'Solo indie developer working on a roguelike adventure.',               TRUE),
('ella_craft',   'ella.craft@example.com',   'ella222',        'IndieQuest-LocalData/user/11/profile.jpg', 'Voxel artist and 3D modeler specializing in low-poly aesthetics.',      TRUE),
('sam_build',    'sam.build@example.com',    'sam333',         'IndieQuest-LocalData/user/12/profile.jpg', 'Systems programmer interested in engine development.',                  FALSE),
('iris_pixel',   'iris.pixel@example.com',   'iris444',        'IndieQuest-LocalData/user/13/profile.jpg', 'Pixel animator creating frame-by-frame sprite sheets.',                TRUE),
('kai_studio',   'kai.studio@example.com',   'kai555',         'IndieQuest-LocalData/user/14/profile.jpg', 'Sound designer and composer for indie games.',                         FALSE),
('rex_gamejam',  'rex.gamejam@example.com',  'rex666',         'IndieQuest-LocalData/user/15/profile.jpg', 'Veteran game jammer with 30+ jam entries under his belt.',             TRUE);

-- 2. Inserción de Publicaciones (25 posts para probar paginación con pageSize=10: 3 páginas)
INSERT INTO "Post" (postTitle, mediaContent, Description)
VALUES 
('First Post',          'IndieQuest-LocalData/postdata/1/media.jpg',  'This is the first post.'),
('Second Post',         'IndieQuest-LocalData/postdata/2/media.jpg',  'This is the second post.'),
('Third Post',          'IndieQuest-LocalData/postdata/3/media.jpg',  'This is the third post.'),
('Pixel Art Basics',    'IndieQuest-LocalData/postdata/4/media.jpg',  'Introduction to pixel art techniques for beginners.'),
('Game Jam Entry',      'IndieQuest-LocalData/postdata/5/media.jpg',  'My submission for the 48-hour game jam.'),
('Unity Tips & Tricks', 'IndieQuest-LocalData/postdata/6/media.jpg',  'Top 5 Unity tips every indie developer should know.'),
('Retro Sound Design',  'IndieQuest-LocalData/postdata/7/media.jpg',  'How to create authentic 8-bit sound effects.'),
('UI Mockups Vol. 1',   'IndieQuest-LocalData/postdata/8/media.jpg',  'Early UI mockups for the new project HUD.'),
('Character Sprites',   'IndieQuest-LocalData/postdata/9/media.jpg',  'Animated character sprite sheet showcase.'),
('Level Design 101',    'IndieQuest-LocalData/postdata/10/media.jpg', 'Core principles of engaging level design.'),
('Devlog #1',           'IndieQuest-LocalData/postdata/11/media.jpg', 'First weekly development log entry.'),
('Devlog #2',           'IndieQuest-LocalData/postdata/12/media.jpg', 'Progress update on the main game mechanic.'),
('Color Palette Guide', 'IndieQuest-LocalData/postdata/13/media.jpg', 'How to choose a harmonious color palette for your game.'),
('Forest Tileset WIP',  'IndieQuest-LocalData/postdata/14/media.jpg', 'Work-in-progress tileset for the forest biome.'),
('Boss Fight Design',   'IndieQuest-LocalData/postdata/15/media.jpg', 'Designing challenging but fair boss encounters.'),
('Main Menu Music',     'IndieQuest-LocalData/postdata/16/media.jpg', 'Background music loop created for the main menu.'),
('Trailer First Cut',   'IndieQuest-LocalData/postdata/17/media.jpg', 'First rough cut of the official game trailer.'),
('Open Source Tools',   'IndieQuest-LocalData/postdata/18/media.jpg', 'Essential free tools every indie developer should know.'),
('Game Feel Tips',      'IndieQuest-LocalData/postdata/19/media.jpg', 'Small tweaks that drastically improve game feel.'),
('Particle Systems',    'IndieQuest-LocalData/postdata/20/media.jpg', 'Adding juice and polish with particle effects.'),
('Isometric Renderer',  'IndieQuest-LocalData/postdata/21/media.jpg', 'Building an isometric map renderer from scratch.'),
('AI Pathfinding',      'IndieQuest-LocalData/postdata/22/media.jpg', 'Implementing A* algorithm for enemy navigation.'),
('Save System Design',  'IndieQuest-LocalData/postdata/23/media.jpg', 'Simple and robust save/load system for 2D games.'),
('Monetization Ideas',  'IndieQuest-LocalData/postdata/24/media.jpg', 'Ethical monetization models for indie games.'),
('Beta Release Notes',  'IndieQuest-LocalData/postdata/25/media.jpg', 'Public beta is now available — patch notes and known issues.');

-- 3. Inserción de Etiquetas
INSERT INTO "Tag" (tagName)
VALUES 
('Software'),
('Design'),
('Content'),
('GameDev'),
('PixelArt'),
('Audio'),
('UI'),
('Devlog'),
('Unity'),
('OpenSource');

-- 4. Relación Makes_MadeBy (distribuidos entre los 15 usuarios)
INSERT INTO "Makes_MadeBy" (idUser, idPost)
VALUES 
(1, 1),  (2, 2),  (3, 3),  (4, 4),  (5, 5),
(6, 6),  (7, 7),  (8, 8),  (9, 9),  (10, 10),
(11, 11),(12, 12),(13, 13),(14, 14),(15, 15),
(1, 16), (2, 17), (3, 18), (4, 19), (5, 20),
(6, 21), (7, 22), (8, 23), (9, 24), (10, 25);

-- 5. Relación Has_Tag (cada post tiene al menos una etiqueta)
INSERT INTO "Has_Tag" (idPost, idTag)
VALUES 
(1, 1),  (2, 2),  (3, 3),  (4, 5),  (5, 4),
(6, 9),  (7, 6),  (8, 7),  (9, 5),  (10, 4),
(11, 8), (12, 8), (13, 2), (14, 5), (15, 4),
(16, 6), (17, 4), (18, 10),(19, 4), (20, 4),
(21, 4), (22, 1), (23, 1), (24, 4), (25, 4);