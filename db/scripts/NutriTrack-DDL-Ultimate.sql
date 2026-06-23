CREATE TYPE rol_usuario AS ENUM ('administrador', 'asesor técnico', 'encargado de campo');
CREATE TYPE tipo_evento AS ENUM ('vacunacion', 'desparasitacion', 'antibiotico', 'cirugia', 'otro');
CREATE TYPE unidad_dosis AS ENUM ('ml', 'l', 'mg', 'g', 'UI', 'cm3');

CREATE TABLE Usuario (
    id_usuario SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    correo VARCHAR(150) UNIQUE NOT NULL,
    rol rol_usuario NOT NULL,
    contrasenia VARCHAR(255) NOT NULL
);

CREATE TABLE Rodeo (
    id_rodeo SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT
);

CREATE TABLE Animal (
    id_animal SERIAL PRIMARY KEY,
    caravana_cuig VARCHAR(5) NOT NULL,
    caravana_nro_manejo VARCHAR(5) NOT NULL,
    fecha_nacimiento DATE,
    peso_al_nacer DECIMAL,
    id_madre INTEGER REFERENCES Animal(id_animal),
    id_padre INTEGER REFERENCES Animal(id_animal),
    raza VARCHAR(100),
    sexo VARCHAR(10),
    color_pelaje VARCHAR(100),
    fecha_alta DATE,
    estado BOOLEAN,
    id_rodeo INTEGER REFERENCES Rodeo(id_rodeo),
    UNIQUE (caravana_cuig, caravana_nro_manejo)
);

CREATE TABLE Medicamento (
    id_medicamento SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT
);

CREATE TABLE EventoSanitario (
    id_evento_sanitario SERIAL PRIMARY KEY,
    tipo_de_evento tipo_evento,
    vigencia_hasta DATE,
    fecha_evento DATE,
    fecha_proxima_aplicacion DATE,
    id_usuario INTEGER REFERENCES Usuario(id_usuario),
    id_animal INTEGER REFERENCES Animal(id_animal)
);

CREATE TABLE DetalleMedicamento (
    id_detalle_medicamento SERIAL PRIMARY KEY,
    dosis DECIMAL,
    unidad unidad_dosis,
    id_evento_sanitario INTEGER REFERENCES EventoSanitario(id_evento_sanitario),
    id_medicamento INTEGER REFERENCES Medicamento(id_medicamento)
);

CREATE TABLE ControlDePeso (
    id_control_de_peso SERIAL PRIMARY KEY,
    fecha_pesaje DATE,
    peso_kg DECIMAL,
    observaciones TEXT,
    id_usuario INTEGER REFERENCES Usuario(id_usuario),
    id_animal INTEGER REFERENCES Animal(id_animal)
);

CREATE TABLE Ingrediente (
    id_ingrediente SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    minerales TEXT,
    energia_metabolizable DECIMAL,
    proteina_bruta DECIMAL,
    fibra_det_neutro DECIMAL,
    unidad_medida VARCHAR(50),
    aditivos TEXT,
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE PlanAlimenticio (
    id_plan_alimenticio SERIAL PRIMARY KEY,
    nombre_plan VARCHAR(100) NOT NULL UNIQUE,
    categoria VARCHAR(50),
    peso_vivo_inicial_promedio DECIMAL,
    peso_objetivo DECIMAL,
    ganancia_peso_esperada DECIMAL,
    tipo_alimentacion VARCHAR(50) NOT NULL,
    tiempo_alimentacion VARCHAR(100),
    kg_ms_diaria_por_animal DECIMAL NOT NULL,
    observaciones TEXT
);

CREATE TABLE PlanAlimenticioDetalle (
    id_plan_alimenticio_detalle SERIAL PRIMARY KEY,
    porcentaje_inclusion_ms DECIMAL,
    observaciones TEXT,
    id_plan_alimenticio INTEGER REFERENCES PlanAlimenticio(id_plan_alimenticio),
    id_ingrediente INTEGER REFERENCES Ingrediente(id_ingrediente)
);

CREATE TABLE PlanRodeoAsignacion (
    id_asignacion_rodeo SERIAL PRIMARY KEY,
    vigencia_desde DATE NOT NULL,
    vigencia_hasta DATE,
    activo BOOLEAN NOT NULL DEFAULT TRUE,
    id_plan_alimenticio INTEGER REFERENCES PlanAlimenticio(id_plan_alimenticio),
    id_rodeo INTEGER REFERENCES Rodeo(id_rodeo)
);
