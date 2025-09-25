SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";

--
-- Base de datos: `inmobiliaria`
--
CREATE DATABASE IF NOT EXISTS `inmobiliaria` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `inmobiliaria`;

--
-- Estructura de tabla para la tabla `contratos`
--
DROP TABLE IF EXISTS `contratos`;
CREATE TABLE `contratos` (
  `id_contrato` int NOT NULL,
  `id_inquilino` int NOT NULL,
  `id_inmueble` int NOT NULL,
  `id_usuario_creador` int NOT NULL,
  `id_usuario_finalizador` int DEFAULT NULL,
  `fecha_inicio` date NOT NULL,
  `fecha_fin_original` date NOT NULL,
  `fecha_fin_anticipada` date DEFAULT NULL,
  `monto_mensual` decimal(12,2) NOT NULL,
  `estado` enum('vigente','finalizado','rescindido') COLLATE utf8mb4_general_ci DEFAULT 'vigente',
  `multa` decimal(12,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Estructura de tabla para la tabla `inquilinos`
--
DROP TABLE IF EXISTS `inquilinos`;
CREATE TABLE `inquilinos` (
  `id_inquilino` int NOT NULL,
  `dni` varchar(20) COLLATE utf8mb4_general_ci NOT NULL,
  `nombre` varchar(100) COLLATE utf8mb4_general_ci NOT NULL,
  `apellido` varchar(100) COLLATE utf8mb4_general_ci NOT NULL,
  `telefono` varchar(50) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `email` varchar(100) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `direccion` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `activo` tinyint DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilinos`
--
INSERT INTO `inquilinos` (`id_inquilino`, `dni`, `nombre`, `apellido`, `telefono`, `email`, `direccion`, `activo`) VALUES
(1, '10663789', 'Susana', 'García', '26660896456', 'susanagarcia@mail.com', 'San Martín 789', 1),
(2, '28805905', 'Daniel', 'Pallero', '26660852321', 'danielpallero@mail.com', 'Mitre 1554', 1),
(3, '35789412', 'Juan Carlos', 'Olmedo', '26650899661', 'juancarlosolmedo@mail.com', 'La Madrid 158', 1),
(4, '42889455', 'Viviana', 'Silva', '26650834551', 'vivianasilva@mail.com', 'Aviador Franco 577', 1),
(5, '42889577', 'Luis', 'Peralta', '2664036228', 'luisperalta@mail.com', 'Mendoza 1238', 1),
(6, '42866667', 'Miguel', 'Ramos', '2664033428', 'miguelramos@mail.com', 'Mendoza 36', 1),
(7, '28126758', 'Lorena', 'Camargo', '2666676228', 'lorenacamargo@mail.com', 'Río Primero 238', 1),
(8, '19125874', 'Felipe', 'Gallo', '2664038898', 'felipegallo@mail.com', '9 de julio 355', 1),
(9, '36974124', 'Orlando', 'Funes', '2664045110', 'orlandofunes@mail.com', 'Santa Fe 655', 1),
(10, '40112974', 'Marianela', 'Hidalgo', '2665036733', 'marianelahidalgo@mail.com', 'Santa Rosa 551', 1),
(11, '12154474', 'virginia', 'Carreras', '2664037812', 'virginiacarreras@mail.com', 'Córdoba 665', 1),
(12, '90125125', 'Soledad', 'Sánchez', '2664236228', 'soledadsanchez@mail.com', 'Catamarca 1544', 1);

--
-- Estructura de tabla para la tabla `pagos`
--
DROP TABLE IF EXISTS `pagos`;
CREATE TABLE `pagos` (
  `id_pago` int NOT NULL,
  `id_contrato` int NOT NULL,
  `numero_pago` int NOT NULL,
  `fecha_vencimiento` date NOT NULL,
  `fecha_pago` date DEFAULT NULL,
  `detalle` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `importe` decimal(12,2) NOT NULL,
  `estado` enum('pendiente','realizado','anulado') COLLATE utf8mb4_general_ci DEFAULT 'pendiente',
  `id_usuario_creador` int NOT NULL,
  `id_usuario_anulador` int DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Estructura de tabla para la tabla `propietarios`
--
DROP TABLE IF EXISTS `propietarios`;
CREATE TABLE `propietarios` (
  `id_propietario` int NOT NULL,
  `dni` varchar(20) COLLATE utf8mb4_general_ci NOT NULL,
  `nombre` varchar(100) COLLATE utf8mb4_general_ci NOT NULL,
  `apellido` varchar(100) COLLATE utf8mb4_general_ci NOT NULL,
  `telefono` varchar(50) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `email` varchar(100) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `direccion` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `activo` tinyint DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietarios`
--
INSERT INTO `propietarios` (`id_propietario`, `dni`, `nombre`, `apellido`, `telefono`, `email`, `direccion`, `activo`) VALUES
(1, '23578966', 'Gustavo', 'Ochoa', '2665064940', 'gustavoochoa@mail.com', 'Lima 890', 1),
(2, '5515237', 'María', 'Lozada', '2665014150', 'marialozada@mail.com', 'Roma 358', 1),
(3, '28189411', 'Carlos Augusto', 'Pereira', '2665015250', 'carlosaugustopereira@mail.com', 'Pasaje Rincón 87', 1),
(4, '28179644', 'Consuelo', 'Pereira', '2665014150', 'consuelopereira@mail.com', 'Pasaje Rincón 87', 1),
(5, '28125411', 'Mariana', 'Lópex', '2664564150', 'marianalopez@mail.com', 'Pasaje Rincón 87', 1),
(6, '37411002', 'Cecilia', 'Amadeo', '2665064250', 'ceciliaamadeo@mail.com', 'Pasaje Rincón 87', 1),
(7, '25741129', 'Claudia', 'Bausili', '2665574360', 'claudiabausili@mail.com', 'Pasaje Rincón 87', 1),
(8, '29114710', 'Daniel', 'Carmona', '2665045697', 'danielcarmona@mail.com', 'Pasaje Rincón 87', 1),
(9, '16574129', 'Silvio', 'Flores', '2665412360', 'silvioflores@mail.com', 'Pasaje Rincón 87', 1),
(10, '12134154', 'Martín', 'Ripoll', '2665014775', 'martinripoll@mail.com', 'Pasaje Rincón 87', 1),
(11, '40574369', 'Pascual', 'Allende', '26651236571', 'pascualallende@mail.com', 'Pasaje Rincón 87', 1);


--
-- Estructura de tabla para la tabla `tipos_inmueble`
--
DROP TABLE IF EXISTS `tipos_inmueble`;
CREATE TABLE `tipos_inmueble` (
  `id_tipo` int NOT NULL,
  `nombre` varchar(100) COLLATE utf8mb4_general_ci NOT NULL,
  `descripcion` text COLLATE utf8mb4_general_ci,
  `activo` tinyint DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tipos_inmueble`
--
INSERT INTO `tipos_inmueble` (`id_tipo`, `nombre`, `descripcion`, `activo`) VALUES
(1, 'Departamento', 'Vivienda dentro de un edificio, generalmente con varios pisos y unidades.', 1),
(2, 'Casa', 'Vivienda independiente, puede tener uno o varios pisos, jardín o cochera.', 1),
(3, 'Local comercial', 'Espacio destinado a actividades comerciales, como tiendas o negocios.', 1),
(4, 'Oficina', 'Inmueble diseñado para actividades administrativas y profesionales.', 1);

--
-- Estructura de tabla para la tabla `usuarios`
--
DROP TABLE IF EXISTS `usuarios`;
CREATE TABLE `usuarios` (
  `id_usuario` int NOT NULL,
  `email` varchar(100) COLLATE utf8mb4_general_ci NOT NULL,
  `contrasena` varchar(255) COLLATE utf8mb4_general_ci NOT NULL,
  `nombre` varchar(100) COLLATE utf8mb4_general_ci NOT NULL,
  `apellido` varchar(100) COLLATE utf8mb4_general_ci NOT NULL,
  `rol` enum('administrador','empleado') COLLATE utf8mb4_general_ci NOT NULL DEFAULT 'empleado',
  `avatar` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `fecha_creacion` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_actualizacion` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `activo` tinyint(1) DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuarios`
--
INSERT INTO `usuarios` (`id_usuario`, `email`, `contrasena`, `nombre`, `apellido`, `rol`, `avatar`, `fecha_creacion`, `fecha_actualizacion`, `activo`) VALUES
(1, 'administrador@mail.com', 'AQAAAAIAAYagAAAAEAI7vDzjljLddAK4n9tI3jkvoi2qTG6x8M6xnZ4JMDi/tn/WLtyd/EVn2qJ6M8pzVA==', 'Federico', 'González', 'administrador', NULL, '2025-08-16 23:47:47', '2025-08-17 19:15:35', 1),
(2, 'usuario1@mail.com', 'AQAAAAIAAYagAAAAEAI7vDzjljLddAK4n9tI3jkvoi2qTG6x8M6xnZ4JMDi/tn/WLtyd/EVn2qJ6M8pzVA==', 'Blas', 'Haberland', 'empleado', NULL, '2025-08-17 18:56:00', '2025-08-19 15:53:57', 1),
(3, 'usuario2@mail.com', 'AQAAAAIAAYagAAAAEAI7vDzjljLddAK4n9tI3jkvoi2qTG6x8M6xnZ4JMDi/tn/WLtyd/EVn2qJ6M8pzVA==', 'Facundo', 'Bacaicoa', 'empleado', NULL, '2025-08-17 19:02:11', '2025-08-19 15:54:01', 1);

--
-- Estructura de tabla para la tabla `inmuebles`
--
DROP TABLE IF EXISTS `inmuebles`;
CREATE TABLE `inmuebles` (
  `id_inmueble` int NOT NULL,
  `id_propietario` int NOT NULL,
  `id_tipo` int NOT NULL,
  `uso` enum('residencial','comercial') COLLATE utf8mb4_general_ci NOT NULL,
  `direccion` varchar(255) COLLATE utf8mb4_general_ci NOT NULL,
  `cantidad_ambientes` int DEFAULT NULL,
  `coordenadas` varchar(100) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `precio` decimal(12,2) NOT NULL,
  `estado` enum('disponible','suspendido','ocupado') COLLATE utf8mb4_general_ci DEFAULT 'disponible',
  `activo` tinyint DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO `inmuebles` (`id_inmueble`, `id_propietario`, `id_tipo`, `uso`, `direccion`, `cantidad_ambientes`, `coordenadas`, `precio`, `estado`, `activo`) VALUES
(1, 2, 1, 'residencial', 'Calle Falsa 123', 2, '34.6037,-58.3816', 200000.00, 'disponible', 1),
(2, 2, 1, 'residencial', 'Avenida Siempre Viva 742', 4, '34.6090,-58.3845', 175000.00, 'disponible', 1),
(3, 3, 3, 'comercial', 'Boulevard Central 456', 1, '34.6118,-58.4173', 1200000.00, 'disponible', 1),
(4, 3, 2, 'residencial', 'Roma 1587', 3, NULL, 380000.00, 'disponible', 1),
(5, 4, 2, 'residencial', 'Aviador Franco 15', 4, NULL, 42000.00, 'disponible', 1),
(6, 5, 2, 'residencial', 'San Francisco 788', 3, NULL, 390000.00, 'disponible', 1),
(7, 6, 3, 'comercial', 'Mendoza 56', 1, NULL, 521000.00, 'disponible', 1),
(8, 9, 4, 'residencial', 'Pje. Don Bosco 56', 3, NULL, 650000.00, 'disponible', 1),
(9, 11, 4, 'comercial', 'Moreno 157', 2, NULL, 455000.00, 'disponible', 1),
(10, 8, 1, 'residencial', 'Ovidio Lagos 157', 2, NULL, 220000.00, 'disponible', 1),
(11, 9, 1, 'residencial', 'Rivera Indarte 688', 2, NULL, 205000.00, 'disponible', 1),
(12, 10, 3, 'comercial', 'Viedma 874', 1, NULL, 453000.00, 'suspendido', 1);
--
-- Indices de la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD PRIMARY KEY (`id_contrato`),
  ADD KEY `id_inquilino` (`id_inquilino`),
  ADD KEY `id_inmueble` (`id_inmueble`),
  ADD KEY `id_usuario_creador` (`id_usuario_creador`),
  ADD KEY `id_usuario_finalizador` (`id_usuario_finalizador`);

--
-- Indices de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`id_inmueble`),
  ADD KEY `id_propietario` (`id_propietario`),
  ADD KEY `id_tipo` (`id_tipo`);

--
-- Indices de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`id_inquilino`),
  ADD UNIQUE KEY `dni` (`dni`);

--
-- Indices de la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`id_pago`),
  ADD KEY `id_contrato` (`id_contrato`),
  ADD KEY `id_usuario_creador` (`id_usuario_creador`),
  ADD KEY `id_usuario_anulador` (`id_usuario_anulador`);

--
-- Indices de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`id_propietario`),
  ADD UNIQUE KEY `dni` (`dni`);

--
-- Indices de la tabla `tipos_inmueble`
--
ALTER TABLE `tipos_inmueble`
  ADD PRIMARY KEY (`id_tipo`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`id_usuario`),
  ADD UNIQUE KEY `email` (`email`);

--
-- AUTO_INCREMENT de la tabla `contratos`
--
ALTER TABLE `contratos`
  MODIFY `id_contrato` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `id_inmueble` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `id_inquilino` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `pagos`
--
ALTER TABLE `pagos`
  MODIFY `id_pago` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `id_propietario` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `tipos_inmueble`
--
ALTER TABLE `tipos_inmueble`
  MODIFY `id_tipo` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `id_usuario` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- Filtros para la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `contratos_ibfk_1` FOREIGN KEY (`id_inquilino`) REFERENCES `inquilinos` (`id_inquilino`),
  ADD CONSTRAINT `contratos_ibfk_2` FOREIGN KEY (`id_inmueble`) REFERENCES `inmuebles` (`id_inmueble`),
  ADD CONSTRAINT `contratos_ibfk_3` FOREIGN KEY (`id_usuario_creador`) REFERENCES `usuarios` (`id_usuario`),
  ADD CONSTRAINT `contratos_ibfk_4` FOREIGN KEY (`id_usuario_finalizador`) REFERENCES `usuarios` (`id_usuario`);

--
-- Filtros para la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `inmuebles_ibfk_1` FOREIGN KEY (`id_propietario`) REFERENCES `propietarios` (`id_propietario`),
  ADD CONSTRAINT `inmuebles_ibfk_2` FOREIGN KEY (`id_tipo`) REFERENCES `tipos_inmueble` (`id_tipo`);

--
-- Filtros para la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `pagos_ibfk_1` FOREIGN KEY (`id_contrato`) REFERENCES `contratos` (`id_contrato`),
  ADD CONSTRAINT `pagos_ibfk_2` FOREIGN KEY (`id_usuario_creador`) REFERENCES `usuarios` (`id_usuario`),
  ADD CONSTRAINT `pagos_ibfk_3` FOREIGN KEY (`id_usuario_anulador`) REFERENCES `usuarios` (`id_usuario`);
COMMIT;
