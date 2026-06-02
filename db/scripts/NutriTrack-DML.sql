-- ENUMs ya creados, no necesitan inserts

-- rodeo
INSERT INTO rodeo (nombre, descripcion) VALUES
('Rodeo Norte', 'Rodeo ubicado en el sector norte de la estancia'),
('Rodeo Sur', 'Rodeo ubicado en el sector sur de la estancia'),
('Rodeo Central', 'Rodeo principal de la estancia');

-- usuario
INSERT INTO usuario (nombre, correo, rol, contrasenia) VALUES
('Juan Pérez', 'juan@estancia.com', 'administrador', '$2b$10$hashadmin123'),
('María Gómez', 'maria@estancia.com', 'asesor técnico', '$2b$10$hashvet456'),
('Carlos López', 'carlos@estancia.com', 'encargado de campo', '$2b$10$hashop789');

-- ingrediente
INSERT INTO ingrediente (nombre, descripcion, minerales, energia_metabolizada, proteina_bruta, fibra_det_neutro, unidad_medida, aditivos) VALUES
('Maíz molido', 'Grano de maíz molido grueso', 0.3, 3.2, 8.5, 9.0, 'kg', NULL),
('Heno de alfalfa', 'Alfalfa de primer corte', 1.2, 2.1, 18.0, 35.0, 'kg', NULL),
('Sorgo granífero', 'Sorgo de alta energía', 0.4, 3.0, 9.0, 8.5, 'kg', NULL),
('Expeller de soja', 'Subproducto de extracción de aceite de soja', 0.6, 2.8, 44.0, 12.0, 'kg', 'antioxidante');

-- medicamento
INSERT INTO medicamento (nombre, descripcion) VALUES
('Ivermectina', 'Antiparasitario de amplio espectro'),
('Oxitetraciclina', 'Antibiótico de amplio espectro'),
('Vitamina AD3E', 'Complejo vitamínico'),
('Vacuna Aftosa', 'Vacuna bivalente contra fiebre aftosa');

-- animal (primero sin madre/padre, luego con referencias)
INSERT INTO animal (caravana_cuig, caravana_nro_manejo, fecha_nacimiento, peso_al_nacer, id_madre, id_padre, raza, sexo, color_pelaje, fecha_alta, estado, id_rodeo) VALUES
('AR001', '00001', '2019-03-15', 38.0, NULL, NULL, 'Angus', 'hembra', 'negro', '2019-03-15', true, 1),
('AR001', '00002', '2018-06-20', 42.0, NULL, NULL, 'Hereford', 'macho', 'colorado', '2018-06-20', true, 1),
('AR002', '00003', '2020-01-10', 36.0, NULL, NULL, 'Angus', 'hembra', 'negro', '2020-01-10', true, 2),
('AR002', '00004', '2017-09-05', 45.0, NULL, NULL, 'Brahman', 'macho', 'gris', '2017-09-05', true, 2);

-- animales con madre y padre referenciados
INSERT INTO animal (caravana_cuig, caravana_nro_manejo, fecha_nacimiento, peso_al_nacer, id_madre, id_padre, raza, sexo, color_pelaje, fecha_alta, estado, id_rodeo) VALUES
('AR003', '00005', '2022-08-12', 34.0, 1, 2, 'Angus', 'hembra', 'negro', '2022-08-12', true, 3),
('AR003', '00006', '2023-02-28', 37.0, 3, 4, 'Brahman', 'macho', 'gris', '2023-02-28', true, 3);

-- evento_sanitario
INSERT INTO evento_sanitario (tipo_de_evento, vigencia_hasta, fecha_evento, fecha_proxima_aplicacion, id_usuario, id_animal) VALUES
('vacunacion', '2025-06-01', '2024-06-01', '2025-06-01', 2, 1),
('desparasitacion', '2024-12-01', '2024-06-01', '2024-12-01', 2, 2),
('vacunacion', '2025-06-15', '2024-06-15', '2025-06-15', 2, 3),
('antibiotico', NULL, '2024-07-10', NULL, 2, 4);

-- control_de_peso - (YA SE PEUDE PROBAR CON LA API)
INSERT INTO control_de_peso (fecha_pesaje, peso_kg, observaciones, id_usuario, id_animal) VALUES
('2024-01-15', 320.5, 'Animal en buen estado', 3, 1),
('2024-01-15', 480.0, NULL, 3, 2),
('2024-02-20', 295.0, 'Leve baja de peso, monitorear', 3, 3),
('2024-03-10', 510.0, NULL, 3, 4),
('2024-04-05', 180.0, 'Ternera en desarrollo normal', 3, 5);

-- plan_alimenticio
INSERT INTO plan_alimenticio (nombre_plan, fecha_plan, categoria, peso_vivo_inicial, peso_objetivo, vigencia_desde, vigencia_hasta, cantidad_animales, observaciones, id_rodeo) VALUES
('Plan engorde novillos Q1', '2024-01-01', 'novillo', 350.0, 480.0, '2024-01-01', '2024-06-30', 15, 'Plan engorde primer semestre', 1),
('Plan mantenimiento vacas', '2024-01-01', 'vaca', 420.0, 450.0, '2024-01-01', '2024-12-31', 20, 'Plan mantenimiento vacas', 2),
('Plan destete terneros', '2024-03-01', 'ternero', 80.0, 180.0, '2024-03-01', '2024-09-30', 10, 'Plan destete y desarrollo', 3);

-- plan_alimenticio_detalle
INSERT INTO plan_alimenticio_detalle (porcentaje_inclusion_ms, cantidad_diaria_kg_ms, observaciones, id_plan_alimenticio, id_ingrediente) VALUES
(60.0, 7.2, NULL, 1, 1),
(40.0, 4.8, NULL, 1, 2),
(50.0, 5.0, NULL, 2, 2),
(50.0, 5.0, NULL, 2, 3),
(70.0, 2.8, 'Base principal del plan', 3, 1),
(30.0, 1.2, NULL, 3, 4);

-- detalle_medicamento
INSERT INTO detalle_medicamento (dosis, unidad, id_evento_sanitario, id_medicamento) VALUES
(5.0, 'ml', 1, 4),
(2.5, 'ml', 2, 1),
(10.0, 'ml', 3, 4),
(15.0, 'ml', 4, 2);