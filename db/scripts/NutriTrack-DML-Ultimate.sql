-- NutriTrack: datos de prueba para el DDL actualizado (5/10/2026)
-- Correr despues del script de esquema, sobre una base RECIEN CREADA: los id de las FK
-- estan escritos a mano y suponen que cada tabla arranca en 1.
--
-- Fechas: eventos sanitarios, pesajes y asignaciones usan CURRENT_DATE +/- N dias, asi los
-- reportes y los vencimientos tienen datos el dia que se cargue el script, no solo hoy.
--
-- Todo va en una transaccion: si una sentencia falla, no queda nada cargado a medias.

BEGIN;

-- ---------------------------------------------------------------------------
-- Usuario
-- rol: nombre exacto del enum RolUsuario (Administrador, AsesorTecnico, EncargadoDeCampo).
-- contrasenia: REEMPLAZAR por hashes reales generados por la API; con estos valores el login no funciona.
-- id 4: usuario dado de baja (CU23). Figura como responsable de registros viejos.
-- Usuarios y contraseñas
-- jperez - juana1234
-- mgomez - maria1234
-- pfuentes - pablo1234
-- aruiz -- aruiz1234
-- ---------------------------------------------------------------------------
INSERT INTO Usuario (nombre, correo, nombre_usuario, rol, contrasenia, activo) VALUES
('Juana Pérez', 'juana@estancia.com', 'jperez', 'Administrador', '$argon2id$v=19$m=65536,t=3,p=1$WeyE5sqChOFX8E0uAhtCZA$NNMlUilSR+9JlCRsdnPYam2aLTEhZQkeBdsIAYTDNFg', true),
('María Gómez', 'maria@estancia.com', 'mgomez', 'AsesorTecnico', '$argon2id$v=19$m=65536,t=3,p=1$OaTdCVLKwYGSaz1dvIZTrQ$e55x+WtjeIvztse9UBvaUraayQ2TvLpSYuqb5rUCkAY', true),
('Pablo Fuentes', 'pablo@estancia.com', 'pfuentes', 'EncargadoDeCampo', '$argon2id$v=19$m=65536,t=3,p=1$7R+22o3yxvD+jvWd6hqmDQ$j8A5i2sO+v5cvetmFmWYhGAJcrIFepNWF3taGQk/J9A', true),
('Ana Ruiz', 'ana@estancia.com', 'aruiz', 'EncargadoDeCampo', '$argon2id$v=19$m=65536,t=3,p=1$aVGnAsrHhoTE3VoAZgy9nQ$8FiJmZZBuwVzRukrxdaEqPCu9SEASWzwCJk71vT0XDE', false);

-- ---------------------------------------------------------------------------
-- Rodeo
-- id 4: rodeo eliminado (CU8). Sin animales y con su asignacion cerrada.
-- ---------------------------------------------------------------------------
INSERT INTO Rodeo (nombre, descripcion, activo) VALUES
('Rodeo Norte', 'Rodeo ubicado en el sector norte de la estancia', true),
('Rodeo Sur', 'Rodeo ubicado en el sector sur de la estancia', true),
('Rodeo Central', 'Rodeo principal de la estancia', true),
('Rodeo Oeste', 'Rodeo dado de baja', false);

-- ---------------------------------------------------------------------------
-- Animal
-- ---------------------------------------------------------------------------
-- id 1 a 4: sin madre ni padre
INSERT INTO Animal (caravana_cuig, caravana_nro_manejo, fecha_nacimiento, peso_al_nacer, id_madre, id_padre, raza, sexo, color_pelaje, fecha_alta, estado, id_rodeo) VALUES
('AR001', '00001', '2019-03-15', 38.0, NULL, NULL, 'Angus', 'Hembra', 'negro', '2019-03-15', true, 1),
('AR001', '00002', '2018-06-20', 42.0, NULL, NULL, 'Hereford', 'Macho', 'colorado', '2018-06-20', true, 1),
('AR002', '00003', '2020-01-10', 36.0, NULL, NULL, 'Angus', 'Hembra', 'negro', '2020-01-10', true, 2),
('AR002', '00004', '2017-09-05', 45.0, NULL, NULL, 'Brahman', 'Macho', 'gris', '2017-09-05', true, 2);

-- id 5 y 6: con madre y padre
INSERT INTO Animal (caravana_cuig, caravana_nro_manejo, fecha_nacimiento, peso_al_nacer, id_madre, id_padre, raza, sexo, color_pelaje, fecha_alta, estado, id_rodeo) VALUES
('AR003', '00005', '2022-08-12', 34.0, 1, 2, 'Angus', 'Hembra', 'negro', '2022-08-12', true, 3),
('AR003', '00006', '2023-02-28', 37.0, 3, 4, 'Brahman', 'Macho', 'gris', '2023-02-28', true, 3);

-- id 7 y 8: con rodeo. El 8 esta inactivo (CU4) y conserva su rodeo y su historial.
INSERT INTO Animal (caravana_cuig, caravana_nro_manejo, fecha_nacimiento, peso_al_nacer, id_madre, id_padre, raza, sexo, color_pelaje, fecha_alta, estado, id_rodeo) VALUES
('AR004', '00007', '2025-03-10', 36.0, NULL, NULL, 'Angus', 'Macho', 'negro', '2025-03-10', true, 1),
('AR004', '00008', '2021-05-18', 39.0, NULL, NULL, 'Hereford', 'Hembra', 'colorado', '2021-05-18', false, 2);

-- id 9 a 12: SIN rodeo (candidatos para crear un rodeo, CU6).
-- 9 y 10 son hijos de los animales 1 (madre) y 2 (padre). El 12 no tiene color de pelaje (opcional).
INSERT INTO Animal (caravana_cuig, caravana_nro_manejo, fecha_nacimiento, peso_al_nacer, id_madre, id_padre, raza, sexo, color_pelaje, fecha_alta, estado, id_rodeo) VALUES
('AR005', '00009', '2024-09-01', 35.0, 1, 2, 'Angus', 'Hembra', 'negro', '2024-09-01', true, NULL),
('AR005', '00010', '2025-08-20', 37.5, 1, 2, 'Angus', 'Macho', 'colorado', '2025-08-20', true, NULL),
('AR006', '00011', '2023-11-05', 40.0, NULL, NULL, 'Brahman', 'Hembra', 'gris', '2023-11-05', true, NULL),
('AR006', '00012', '2024-04-22', 41.0, NULL, NULL, 'Hereford', 'Macho', NULL, '2024-04-22', true, NULL);

-- ---------------------------------------------------------------------------
-- Medicamento
-- id 5 y 6: inactivos (CU31). El 5 se uso en un evento viejo; el 6 nunca se uso.
-- El nombre de un inactivo NO se puede reusar (ux_medicamento_nombres cuenta los inactivos).
-- ---------------------------------------------------------------------------
INSERT INTO Medicamento (nombre, descripcion, activo) VALUES
('Ivermectina', 'Antiparasitario de amplio espectro', true),
('Oxitetraciclina', 'Antibiótico de amplio espectro', true),
('Vitamina AD3E', 'Complejo vitamínico', true),
('Vacuna Aftosa', 'Vacuna bivalente contra fiebre aftosa', true),
('Closantel', 'Antiparasitario; discontinuado en el establecimiento', false),
('Penicilina', 'Antibiótico; discontinuado en el establecimiento', false);

-- ---------------------------------------------------------------------------
-- EventoSanitario
-- tipo_evento: nombre exacto del enum (verificar contra el enum de Core).
-- Conjunto cerrado del CU12: Vacunacion, Desparasitacion, Tratamiento, Refuerzo, Control, Otro.
-- Casos para el reporte de fechas importantes (CU18) y las alertas (CU15, rango de 7 dias):
--   1  proxima aplicacion y vencimiento en 3 dias    -> semanal, y dentro del rango de aviso
--   2  proxima aplicacion y vencimiento en 10 dias   -> quincenal
--   3  proxima aplicacion y vencimiento en 25 dias   -> mensual
--   4  sin fechas a recordar                         -> no aparece en ningun periodo
--   5  vencido hace 20 dias, medicamento inactivo    -> solo en periodos pasados
--   6  proxima aplicacion HOY, sin vigencia          -> borde del periodo; solo en "proximas"
--   7  solo vigencia (6 dias), sin medicamentos      -> solo en "vencimientos"
--   8  animal INACTIVO, responsable INACTIVO         -> definir si el reporte lo muestra
--   9  animal SIN rodeo, fechas a 45 y 60 dias       -> fuera del mes; no sale filtrando por rodeo
--   10 dos medicamentos, a 14 y 30 dias              -> bordes de quincenal y mensual
-- ---------------------------------------------------------------------------
INSERT INTO EventoSanitario (tipo_evento, fecha_evento, vigencia_hasta, fecha_proxima_aplicacion, observaciones, id_usuario, id_animal) VALUES
('Vacunacion',      CURRENT_DATE - 362, CURRENT_DATE + 3,  CURRENT_DATE + 3,  NULL, 2, 1),
('Desparasitacion', CURRENT_DATE - 170, CURRENT_DATE + 10, CURRENT_DATE + 10, NULL, 2, 2),
('Vacunacion',      CURRENT_DATE - 340, CURRENT_DATE + 25, CURRENT_DATE + 25, NULL, 2, 3),
('Tratamiento',     CURRENT_DATE - 20,  NULL,              NULL,              'Tratamiento con antibiótico', 2, 4),
('Desparasitacion', CURRENT_DATE - 200, CURRENT_DATE - 20, CURRENT_DATE - 20, NULL, 2, 5),
('Refuerzo',        CURRENT_DATE - 30,  NULL,              CURRENT_DATE,      'Segunda dosis', 3, 6),
('Control',         CURRENT_DATE - 5,   CURRENT_DATE + 6,  NULL,              'Revisión general previa a la venta', 2, 7),
('Vacunacion',      CURRENT_DATE - 100, NULL,              CURRENT_DATE + 4,  NULL, 4, 8),
('Otro',            CURRENT_DATE - 10,  CURRENT_DATE + 60, CURRENT_DATE + 45, 'Descorne', 3, 9),
('Vacunacion',      CURRENT_DATE - 15,  CURRENT_DATE + 30, CURRENT_DATE + 14, 'Vacuna y refuerzo vitamínico', 1, 1);

-- ---------------------------------------------------------------------------
-- DetalleMedicamento
-- El evento 7 no lleva medicamentos. El 10 lleva dos, uno sin dosis ni unidad (opcionales).
-- unidad: enum UnidadDosis (ml, l, mg, g, UI, cm3).
-- ---------------------------------------------------------------------------
INSERT INTO DetalleMedicamento (dosis, unidad, observaciones, id_evento_sanitario, id_medicamento) VALUES
(5.0,  'ml',  NULL, 1, 4),
(2.5,  'ml',  NULL, 2, 1),
(10.0, 'ml',  NULL, 3, 4),
(15.0, 'ml',  NULL, 4, 2),
(8.0,  'ml',  'Aplicado antes de la baja del medicamento', 5, 5),
(5.0,  'ml',  NULL, 6, 4),
(5.0,  'ml',  NULL, 8, 4),
(20.0, 'mg',  NULL, 9, 2),
(5.0,  'ml',  NULL, 10, 4),
(NULL, NULL,  'Dosis según prospecto', 10, 3);

-- ---------------------------------------------------------------------------
-- ControlDePeso
-- Casos para el reporte de evolucion de peso (CU19) y la alerta por desvio (CU14):
--   Rodeo Norte (plan de engorde, espera 0.8 kg/dia): 1 y 2 ganan menos; 7 gana mas
--   Rodeo Sur (mantenimiento, espera 0.3 kg/dia): 3 cumple; 4 BAJA de peso (variacion negativa)
--   Animal 8 (inactivo): pesajes viejos, cargados por el usuario inactivo
--   Animal 6: un solo pesaje (variacion sin dato)
--   Animal 9: pesaje con fecha de HOY (borde del periodo)
--   Animales 11 y 12: sin pesajes
-- ---------------------------------------------------------------------------
INSERT INTO ControlDePeso (fecha_pesaje, peso_kg, observaciones, id_usuario, id_animal) VALUES
(CURRENT_DATE - 90,  455.0, 'Animal en buen estado', 3, 1),
(CURRENT_DATE - 60,  470.0, NULL, 3, 1),
(CURRENT_DATE - 30,  482.0, NULL, 3, 1),
(CURRENT_DATE - 2,   490.0, NULL, 3, 1),
(CURRENT_DATE - 90,  720.0, NULL, 3, 2),
(CURRENT_DATE - 30,  735.0, NULL, 3, 2),
(CURRENT_DATE - 2,   741.0, NULL, 3, 2),
(CURRENT_DATE - 90,  330.0, NULL, 3, 7),
(CURRENT_DATE - 60,  357.0, NULL, 3, 7),
(CURRENT_DATE - 30,  383.5, NULL, 3, 7),
(CURRENT_DATE - 2,   409.5, 'Buen ritmo de engorde', 3, 7),
(CURRENT_DATE - 75,  440.0, NULL, 3, 3),
(CURRENT_DATE - 40,  452.0, NULL, 3, 3),
(CURRENT_DATE - 7,   463.0, NULL, 1, 3),
(CURRENT_DATE - 75,  810.0, NULL, 3, 4),
(CURRENT_DATE - 40,  805.0, 'Leve baja de peso, monitorear', 3, 4),
(CURRENT_DATE - 7,   798.5, 'Sigue bajando; en tratamiento', 1, 4),
(CURRENT_DATE - 150, 400.0, NULL, 4, 8),
(CURRENT_DATE - 110, 410.0, NULL, 4, 8),
(CURRENT_DATE - 45,  430.0, NULL, 3, 5),
(CURRENT_DATE - 10,  445.0, NULL, 3, 5),
(CURRENT_DATE - 10,  520.0, NULL, 3, 6),
(CURRENT_DATE - 20,  380.0, NULL, 3, 9),
(CURRENT_DATE,       392.0, 'Vaquillona en desarrollo normal', 3, 9),
(CURRENT_DATE - 20,  290.0, NULL, 3, 10);

-- ---------------------------------------------------------------------------
-- Ingrediente
-- unidad_medida: enum UnidadMedida (kg, l, gr, ml, fardo, rollo).
-- id 5: inactivo (CU27) y todavia presente en el plan 4.
-- El nombre de un inactivo SI se puede reusar (ux_ingrediente_nombres filtra por activo).
-- ---------------------------------------------------------------------------
INSERT INTO Ingrediente (nombre, descripcion, minerales, energia_metabolizable, proteina_bruta, fibra_det_neutro, unidad_medida, aditivos, activo) VALUES
('Maíz molido', 'Grano de maíz molido grueso', 'Calcio, Fósforo', 3.2, 8.5, 9.0, 'kg', NULL, true),
('Heno de alfalfa', 'Alfalfa de primer corte', 'Calcio', 2.1, 18.0, 35.0, 'kg', NULL, true),
('Sorgo granífero', 'Sorgo de alta energía', 'Magnesio', 3.0, 9.0, 8.5, 'kg', NULL, true),
('Núcleo mineral', 'Premezcla mineral para bovinos', 'Calcio, Fósforo, Magnesio, Zinc', 0.0, 0.0, 0.0, 'kg', 'antioxidante', true),
('Expeller de soja', 'Subproducto proteico; se dejó de comprar', 'Fósforo', 2.9, 44.0, 14.0, 'kg', NULL, false);

-- ---------------------------------------------------------------------------
-- PlanAlimenticio
-- id 4: sin asignar a ningun rodeo, con todos los opcionales en NULL (CU9 D2 a D5, D7 y D9)
-- y con un ingrediente inactivo: no se puede guardar una edicion hasta quitarlo (CU27 R5).
-- ---------------------------------------------------------------------------
INSERT INTO PlanAlimenticio (nombre_plan, categoria, peso_vivo_inicial_promedio, peso_objetivo, ganancia_peso_esperada, tipo_alimentacion, tiempo_alimentacion, kg_ms_diaria_por_animal, observaciones) VALUES
('Plan engorde novillos Q1', 'novillo', 350.0, 480.0, 0.8, 'corral', '180 dias', 12.0, 'Plan engorde primer semestre'),
('Plan mantenimiento vacas', 'vaca', 420.0, 450.0, 0.3, 'pastura implantada', '365 dias', 8.0, 'Plan mantenimiento vacas'),
('Plan destete terneros', 'ternero', 80.0, 180.0, 0.6, 'campo natural', '180 dias', 4.0, 'Plan destete y desarrollo'),
('Plan recría vaquillonas', NULL, NULL, NULL, NULL, 'pastura implantada', NULL, 7.5, NULL);

-- ---------------------------------------------------------------------------
-- PlanAlimenticioDetalle (cada plan suma 100)
-- ---------------------------------------------------------------------------
INSERT INTO PlanAlimenticioDetalle (porcentaje_inclusion_ms, observaciones, id_plan_alimenticio, id_ingrediente) VALUES
(60.0, 'Base energética del plan', 1, 1),
(30.0, NULL, 1, 2),
(10.0, 'Aporte mineral', 1, 4),
(50.0, NULL, 2, 2),
(50.0, NULL, 2, 3),
(70.0, 'Base principal del plan', 3, 1),
(30.0, 'Aporte mineral', 3, 4),
(60.0, NULL, 4, 2),
(25.0, NULL, 4, 1),
(15.0, 'Ingrediente dado de baja', 4, 5);

-- ---------------------------------------------------------------------------
-- PlanRodeoAsignacion
-- Una sola asignacion activa por rodeo (ux_asignacion_rodeo_activa).
--   1  Rodeo Norte, historica: reemplazada por la 2             -> inactiva
--   2  Rodeo Norte, vigente: vence en 5 dias                    -> alerta de vencimiento (CU16)
--   3  Rodeo Sur, vigente: vence en 165 dias
--   4  Rodeo Central, vigente y sin fecha de fin
--   5  Rodeo Oeste (eliminado): cerrada al dar de baja el rodeo -> inactiva
-- ---------------------------------------------------------------------------
INSERT INTO PlanRodeoAsignacion (vigencia_desde, vigencia_hasta, activo, id_plan_alimenticio, id_rodeo) VALUES
(CURRENT_DATE - 300, CURRENT_DATE - 121, false, 3, 1),
(CURRENT_DATE - 120, CURRENT_DATE + 5,   true,  1, 1),
(CURRENT_DATE - 200, CURRENT_DATE + 165, true,  2, 2),
(CURRENT_DATE - 60,  NULL,               true,  3, 3),
(CURRENT_DATE - 400, CURRENT_DATE - 220, false, 2, 4);

COMMIT;