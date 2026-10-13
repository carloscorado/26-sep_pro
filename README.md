# Análisis y corrección de vulnerabilidades con OWASP ZAP

**Aplicación analizada:** WebEmpresa (Blazor Server, .NET) en `http://localhost:5286`
**Herramienta:** OWASP ZAP 2.17.0 (escaneo pasivo)
**Evidencia:** `evidencia/Informe_ZAP_Antes.html` y `evidencia/Informe_ZAP_Despues.html`

## 1. Resumen global

| Nivel de riesgo | Antes | Después |
|---|---|---|
| Alto | 0 | 0 |
| Medio | 4 tipos de alerta | 1 tipo de alerta |
| Bajo | 1 tipo de alerta | 0 |
| Informativo | 3 tipos de alerta | 3 tipos de alerta |
| **Total de tipos de alerta** | **8** | **4** |

## 2. Tabla comparativa de resultados

| Alerta encontrada | Riesgo | Corrección realizada | Resultado final |
|---|---|---|---|
| CSP: Directiva Wildcard | Medio | Se definió una política CSP explícita con `default-src 'self'`, `script-src 'self'`, `style-src 'self'` y `object-src 'none'`, sin comodines. | Corregida |
| CSP: script-src unsafe-inline | Medio | `script-src 'self'` ya no incluye `unsafe-inline`. | Corregida |
| CSP: style-src unsafe-inline | Medio | `style-src 'self'` ya no incluye `unsafe-inline`. | Corregida |
| CSP: Failure to Define Directive with No Fallback | Medio | Se añadió CSP con `base-uri` y `frame-ancestors`, pero la directiva `form-action` sigue sin definirse (no hereda de `default-src`). | No corregida |
| Falta encabezado X-Content-Type-Options | Bajo | Se agregó el encabezado `X-Content-Type-Options: nosniff` a todas las respuestas. | Corregida |
| Aplicación Web Moderna | Informativo | No requiere modificación. | Informativa |
| Divulgación de información - Comentarios sospechosos | Informativo | No requiere modificación. | Falso positivo |
| Respuesta de Gestión de Sesión Identificada | Informativo | No requiere modificación. | Informativa |

Mejoras adicionales visibles en el segundo informe: `X-Frame-Options` pasó de `SAMEORIGIN` a `DENY` y se agregó `Strict-Transport-Security`.

## 3. Justificación de las alertas que permanecen

**CSP: Failure to Define Directive with No Fallback (Medio).** `form-action` es una de las directivas que no usan `default-src` como respaldo. Si no se define, el navegador permite enviar formularios a cualquier destino. La política actual no la incluye, por eso la alerta persiste. Se corrige agregando `form-action 'self'`.

**Aplicación Web Moderna (Informativo).** ZAP detecta enlaces sin `href` tradicional (el `<a class="navbar-brand" href>` de Blazor). Es una característica del framework y no un riesgo. ZAP mismo indica que no requiere cambios.

**Divulgación de información - Comentarios sospechosos (Informativo, falso positivo).** La regla busca la palabra `USER` y la encontró en un comentario del archivo `ReconnectModal.razor.js`, generado por la plantilla de Blazor: *"We'll reload the page so the user can continue using the app..."*. El comentario no contiene credenciales, rutas internas ni datos sensibles; es solo la palabra "user" en prosa.

**Respuesta de Gestión de Sesión Identificada (Informativo).** ZAP detecta la cookie `.AspNetCore.Antiforgery`, que ASP.NET Core usa para protección CSRF. Es comportamiento esperado y seguro: la cookie tiene `HttpOnly` y `SameSite=Strict`. El atributo `Secure` no aparece porque se prueba sobre HTTP en localhost; en producción con HTTPS se aplica.

## 4. Código aplicado (Program.cs)

```csharp
app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;
    headers["X-Content-Type-Options"] = "nosniff";
    headers["X-Frame-Options"] = "DENY";
    headers["Content-Security-Policy"] =
        "default-src 'self'; script-src 'self'; style-src 'self'; " +
        "object-src 'none'; base-uri 'self'; form-action 'self'; frame-ancestors 'none';";
    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
```

## 5. Evidencia en el repositorio

```
/evidencia/Informe_ZAP_Antes.html
/evidencia/Informe_ZAP_Despues.html
README.md
```