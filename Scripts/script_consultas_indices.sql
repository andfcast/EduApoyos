-- ============================================================================
-- 1. SOLICITUDES PENDIENTES CON MÁS DE 5 DÍAS SIN ACTUALIZACIÓN
-- ============================================================================

SELECT 
    s.Id AS SolicitudId,
    s.EstudianteId,
    u.NombreCompleto AS NombreEstudiante,
    ta.Nombre AS TipoApoyo,
    s.MontoSolicitado,
    s.FechaActualizacion,
    DATEDIFF(DAY, s.FechaActualizacion, GETUTCDATE()) AS DiasSinActualizar
FROM dbo.SolicitudesApoyo s
INNER JOIN dbo.Estudiantes e ON s.EstudianteId = e.Id
INNER JOIN dbo.Usuarios u ON e.UsuarioId = u.Id
INNER JOIN dbo.TiposApoyo ta ON s.TipoApoyoId = ta.Id
WHERE s.EstadoSolicitudId = 1 -- Estado 'Pendiente'
  AND s.FechaActualizacion <= DATEADD(DAY, -5, GETUTCDATE())
ORDER BY s.FechaActualizacion ASC;

GO

-- ============================================================================
-- 2. TOTAL DE SOLICITUDES AGRUPADAS POR ESTADO Y TIPO DE APOYO (ÚLTIMO MES)
-- ============================================================================

SELECT 
    es.Nombre AS EstadoSolicitud,
    ta.Nombre AS TipoApoyo,
    COUNT(s.Id) AS TotalSolicitudes
FROM dbo.SolicitudesApoyo s
INNER JOIN dbo.EstadosSolicitud es ON s.EstadoSolicitudId = es.Id
INNER JOIN dbo.TiposApoyo ta ON s.TipoApoyoId = ta.Id
WHERE s.FechaCreacion >= DATEADD(MONTH, -1, GETUTCDATE())
GROUP BY es.Nombre, ta.Nombre
ORDER BY es.Nombre, ta.Nombre;

GO

-- ============================================================================
-- 3. ÍNDICE NUEVO
-- ============================================================================
-- Se creó este índice para poder faciltar una posterior consulta por el estado 
-- de la solicitud y filtro por fecha de actualización, y como uno sabe que como 
-- usuario quiere ver las más recientes, evitando que se usen operaciones como
-- Sort que son costosas en cuanto a procesamiento y disposición de recursos.

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SolicitudesApoyo_Estado_FechaActualizacion' AND object_id = OBJECT_ID('dbo.SolicitudesApoyo'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_SolicitudesApoyo_Estado_FechaActualizacion
    ON dbo.SolicitudesApoyo (EstadoSolicitudId, FechaActualizacion)
    INCLUDE (EstudianteId, TipoApoyoId, MontoSolicitado);
END

GO