BEGIN TRANSACTION;
-- Table: public.artifacts

-- DROP TABLE IF EXISTS public.artifacts;

CREATE TABLE IF NOT EXISTS public.artifacts
(
    id integer NOT NULL DEFAULT nextval('"Artifacts_Id_seq"'::regclass),
    name character varying COLLATE pg_catalog."default" NOT NULL,
    module character varying COLLATE pg_catalog."default" NOT NULL,
    status character varying COLLATE pg_catalog."default" NOT NULL,
    root boolean NOT NULL DEFAULT false,
    CONSTRAINT "Artifacts_pkey" PRIMARY KEY (id),
    CONSTRAINT "Artifacts_Name_key" UNIQUE (name)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.artifacts
    OWNER to postgres;


-- Table: public.artifact_versions

-- DROP TABLE IF EXISTS public.artifact_versions;

CREATE TABLE IF NOT EXISTS public.artifact_versions
(
    id integer NOT NULL DEFAULT nextval('"ArtifactVersions_Id_seq"'::regclass),
    artifact_id integer NOT NULL,
    version character varying COLLATE pg_catalog."default" NOT NULL,
    location character varying COLLATE pg_catalog."default" NOT NULL,
    status character varying COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "ArtifactVersions_pkey" PRIMARY KEY (id),
    CONSTRAINT "ArtifactId" FOREIGN KEY (artifact_id)
        REFERENCES public.artifacts (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE
        NOT VALID
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.artifact_versions
    OWNER to postgres;

-- Index: fki_ArtifactId

-- DROP INDEX IF EXISTS public."fki_ArtifactId";

CREATE INDEX IF NOT EXISTS "fki_ArtifactId"
    ON public.artifact_versions USING btree
    (artifact_id ASC NULLS LAST)
    TABLESPACE pg_default;

-- Table: public.artifact_dependencies

-- DROP TABLE IF EXISTS public.artifact_dependencies;

CREATE TABLE IF NOT EXISTS public.artifact_dependencies
(
    id integer NOT NULL DEFAULT nextval('"ArtifactDependencies_Id_seq"'::regclass),
    artifact_id integer NOT NULL,
    name character varying COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "ArtifactDependencies_pkey" PRIMARY KEY (id),
    CONSTRAINT "ArtifactId" FOREIGN KEY (artifact_id)
        REFERENCES public.artifacts (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE
        NOT VALID
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.artifact_dependencies
    OWNER to postgres;

COMMIT;
