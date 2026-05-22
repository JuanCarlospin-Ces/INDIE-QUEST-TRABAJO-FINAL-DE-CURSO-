-- 1. Inserción de Usuarios (18 usuarios con datos reales)
INSERT INTO "User" (username, email, password, profilepicture, userbio, availableforwork)
VALUES 
('Manu-PixelArt', 'Martiel@gmail.com', 'heho', 'IndieQuest-LocalData/user/1/Absolute_Cinema_Hee_hoo.jpeg', 'Estudiante de DAM y artista de Pixel Art\n\nPuedes contactar conmigo por: ManuPixel@gmail.com', TRUE),
('juanca', 'juancarlospin@gmail.com', '123123', 'IndieQuest-LocalData/user/2/71pcOzMoXAL._AC_UF1000,1000_QL80_.jpg', 'Esta es mi bio :)\n\nCorreo personal: juancarlospin@gmail.com', TRUE),
('PablitoElMalito', 'sam.build@hotmail.com', 'sam33', 'IndieQuest-LocalData/user/3/images (1).jpg', 'Systems programmer interested in engine development.\n\nContact: pablo3@hotmail.com', FALSE),
('carlos_dev', 'carlosdev@gmail.com', 'carloscarlos', 'IndieQuest-LocalData/user/4/193ff2ca8a4e1a844a2616fa30791ca8.webp', 'Desarrollador indie de videojuegos retro, experto en interfaces graficas\n\nCorreo personal: carlosdev@gmail.com', TRUE),
('NachoIA', 'NachoAI@outlook.com', 'nacho123', 'IndieQuest-LocalData/user/5/gta-6-leonida-keys-screenshots_ehwt.jpg', 'Experto en workflows con agentes de Inteligencia Artificial en videojuegos.\n\nContacto: NachoAI@outlook.com', FALSE),
('SolanaChess', 'SunPiece@correo.com', '123123', 'IndieQuest-LocalData/user/6/pngtree-chess-clipart-isometric-chess-pieces-isolated-on-a-white-background-cartoon-vector-png-image_6809262.png', 'Aficionado del ajedrez\n\nPuedes contactar conmigo en SunPiece@correo.com', TRUE),
('Prueba', 'prueba@gmail.com', '123123', 'IndieQuest-LocalData/user/8/e5612c808168df5e4e680c04d5b05a49.jpg', 'Hola! Soy un usuario de prueba', TRUE),
('AriGameplays', 'Ari@gmail.com', '123123', 'IndieQuest-LocalData/user/9/aripnh.png', 'Existiendo', FALSE),
('GutiFit', 'pgut@hotmail.com', '123123', 'IndieQuest-LocalData/user/10/siluetas-vectoriales-de-logotipos-gimnasio-418465242.webp', 'Especialista en modelado de personajes\n\nPuedes contactar conmigo en: pgut@hotmail.com', TRUE),
('LunaCruzcampo', 'lunaluna@gmail.com', 'cruzcampoforever', 'IndieQuest-LocalData/user/11/cruzcampo_logo_portada.jpg', 'lunaluna@gmail.com', FALSE),
('Pablohoc', 'Pablohoc@gmail.com', 'tuno123', 'IndieQuest-LocalData/user/12/sin-titulo-1_npes.1280.webp', 'Musico profesional\n\nPablohoc@gmail.com', TRUE),
('Rafa', 'rafamalagon@gmail.com', 'rafarafa', 'IndieQuest-LocalData/user/13/Captura de pantalla 2026-05-22 232400.png', 'Diseñador de UIs y especialista en color\n\nrafamalagon@gmail.com', FALSE),
('SergieDev', 'sergio@gmail.com', 'sergio', 'IndieQuest-LocalData/user/14/encrypted-tbn2_gstatic_com-shopping.jpg', 'Especialista en GODOT y desarollo de videojuegos', FALSE),
('GonzaloDev', 'gonzalo@gmail.com', 'gonzalo', NULL, NULL, FALSE),
('Llluc_el_Gran_Borracho', 'lluccolls@gmail.com', 'llucelcopas', 'IndieQuest-LocalData/user/16/copa-de-vino-de-cristal-con-el-vino-rojo-un-realista-transparente-vector-65715620.webp', 'Puedes contactar conmigo tanto para trabajo como para una copita en \n\nlluccolls@gmail.com', TRUE),
('AlvaroTranslate', 'alvarotransalte@gmail.com', '123123', NULL, 'Traductor oficial, si quieres tu proyecto en mas idiomas contacta a \n\nalvarotransalte@gmail.com', TRUE),
('Munioz', 'pabloM@outlook.com', 'pablo', NULL, NULL, FALSE);

-- 2. Inserción de Publicaciones (13 posts con datos reales)
INSERT INTO "Post" (posttitle, mediacontent, description)
VALUES 
('Mi primera sprite sheet', 'IndieQuest-LocalData/postdata/1/Movement - Sheet.png', 'Animaciones diseñadas para mi primer videojuego'),
('Gameplay Cyclops adventure', 'IndieQuest-LocalData/postdata/2/Video Cyclops Adventure.mp4', 'Un juego estilo dinosauro de google chrome'),
('Mi proyecto de ciudad', 'IndieQuest-LocalData/postdata/3/castle_showcase.webp', 'Modelo 3D de ciudad para mi videojuego personal en desarollo'),
('Ajedrez', '', ':)'),
('Motor de ajedrez en ejecución', 'IndieQuest-LocalData/postdata/7/Ajedrez.mp4', 'Demo basica\n'),
('Asset de audio gratuito', 'IndieQuest-LocalData/postdata/8/DANIO_SOUND.mp3', 'Efecto sonoro de daño'),
('Compartiendo assets gratuitos cada dia #1', 'IndieQuest-LocalData/postdata/9/juego.mp3', 'Musica generica de ambientación'),
('WORK IN PROGRESS Modelo 3D', 'IndieQuest-LocalData/postdata/10/4uzxksb0.jpg', 'Estoy trabando en el modelo del protagonista de mi juego en Unity'),
('Diseñando escenarios', 'IndieQuest-LocalData/postdata/11/0528620861b544eeb4af8216053845e3.jpeg', 'Modelado de assets para escenarios'),
('Musica ambientación de aventura', 'IndieQuest-LocalData/postdata/12/musica_aventura.mp3', 'Pa usarla como querais'),
('Objeto para mi videojuego', 'IndieQuest-LocalData/postdata/13/pixelated-mug-of-beer-large-mug-of-beer-drink-icon-pixelated-for-the-pixel-art-game-and-icon-for-website-and-game-old-school-retro-free-vector.jpg', 'Estoy desarrollando los equipamentos de mi juego roguelike, que os parece?');

-- 3. Inserción de Etiquetas (14 tags con datos reales)
INSERT INTO "Tag" (tagname)
VALUES 
('pixelart'),
('gamedev'),
('runner'),
('3dmodeling'),
('blender'),
('godot'),
('api'),
('AI'),
('audio'),
('music'),
('ambience'),
('freeasset'),
('3Dmodels'),
('3DBackgrounds');

-- 4. Relación Makes_MadeBy (usuarios que crean posts)
INSERT INTO "Makes_MadeBy" (iduser, idpost)
VALUES 
(2, 1),
(2, 2),
(3, 3),
(6, 4),
(6, 5),
(8, 6),
(5, 7),
(10, 8),
(11, 9),
(12, 10),
(16, 11);

-- 5. Relación Has_Tag (posts con etiquetas)
INSERT INTO "Has_Tag" (idpost, idtag)
VALUES 
(1, 1),
(2, 2),
(2, 3),
(3, 4),
(3, 5),
(4, 7),
(5, 7),
(5, 8),
(6, 2),
(6, 9),
(7, 10),
(7, 11),
(7, 12),
(8, 5),
(8, 13),
(9, 5),
(9, 14),
(10, 10),
(10, 11),
(11, 1),
(11, 6);