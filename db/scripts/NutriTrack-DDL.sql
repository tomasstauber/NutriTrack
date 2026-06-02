CREATE TYPE rol_usuario AS ENUM ('administrador', 'asesor técnico', 'encargado de campo');
CREATE TYPE sexo_animal AS ENUM ('macho', 'hembra');
CREATE TYPE tipo_evento AS ENUM ('vacunacion', 'desparasitacion', 'antibiotico', 'cirugia', 'otro');
CREATE TYPE unidad_dosis AS ENUM ('ml', 'l', 'mg', 'g', 'UI', 'cm3');
CREATE TYPE categoria_animal AS ENUM ('ternero', 'ternera', 'novillo', 'vaquillona', 'toro', 'vaca');

CREATE TABLE rodeo (
    id_rodeo SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT
);

CREATE TABLE usuario (
    id_usuario SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    correo VARCHAR(150) UNIQUE NOT NULL,
    rol rol_usuario NOT NULL,
    contrasenia VARCHAR(255) NOT NULL
);

CREATE TABLE ingrediente (
    id_ingrediente SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    minerales FLOAT,
    energia_metabolizada FLOAT,
    proteina_bruta FLOAT,
    fibra_det_neutro FLOAT,
    unidad_medida VARCHAR(50),
    aditivos TEXT
);

CREATE TABLE medicamento (
    id_medicamento SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT
);

CREATE TABLE animal (
    id_animal SERIAL PRIMARY KEY,
    caravana_cuig VARCHAR(5) NOT NULL,
    caravana_nro_manejo VARCHAR(5) NOT NULL,
    fecha_nacimiento DATE,
    peso_al_nacer FLOAT,
    id_madre INTEGER REFERENCES animal(id_animal),
    id_padre INTEGER REFERENCES animal(id_animal),
    raza VARCHAR(100),
    sexo sexo_animal,
    color_pelaje VARCHAR(100),
    fecha_alta DATE,
    estado BOOLEAN,
    id_rodeo INTEGER REFERENCES rodeo(id_rodeo),
    UNIQUE (caravana_cuig, caravana_nro_manejo)
);

CREATE TABLE evento_sanitario (
    id_evento_sanitario SERIAL PRIMARY KEY,
    tipo_de_evento tipo_evento,
    vigencia_hasta DATE,
    fecha_evento DATE,
    fecha_proxima_aplicacion DATE,
    id_usuario INTEGER REFERENCES usuario(id_usuario),
    id_animal INTEGER REFERENCES animal(id_animal)
);

CREATE TABLE control_de_peso (
    id_control_de_peso SERIAL PRIMARY KEY,
    fecha_pesaje DATE,
    peso_kg FLOAT,
    observaciones TEXT,
    id_usuario INTEGER REFERENCES usuario(id_usuario),
    id_animal INTEGER REFERENCES animal(id_animal)
);

CREATE TABLE plan_alimenticio (
    id_plan_alimenticio SERIAL PRIMARY KEY,
    nombre_plan VARCHAR(100) NOT NULL UNIQUE,
    fecha_plan DATE,
    categoria categoria_animal,
    peso_vivo_inicial FLOAT,
    peso_objetivo FLOAT,
    vigencia_desde DATE,
    vigencia_hasta DATE,
    cantidad_animales INTEGER,
    observaciones TEXT,
    id_rodeo INTEGER REFERENCES rodeo(id_rodeo)
);

CREATE TABLE plan_alimenticio_detalle (
    id_plan_alimenticio_detalle SERIAL PRIMARY KEY,
    porcentaje_inclusion_ms FLOAT,
    cantidad_diaria_kg_ms FLOAT,
    observaciones TEXT,
    id_plan_alimenticio INTEGER REFERENCES plan_alimenticio(id_plan_alimenticio),
    id_ingrediente INTEGER REFERENCES ingrediente(id_ingrediente)
);

CREATE TABLE detalle_medicamento (
    id_detalle_medicamento SERIAL PRIMARY KEY,
    dosis FLOAT,
    unidad unidad_dosis,
    id_evento_sanitario INTEGER REFERENCES evento_sanitario(id_evento_sanitario),
    id_medicamento INTEGER REFERENCES medicamento(id_medicamento)
);