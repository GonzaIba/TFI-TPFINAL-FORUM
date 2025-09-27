## Context
Buenas, estoy implementando un desarrollo de Proyecto final de ingenieria para sistemas. 
Mi proyecto se basa en una plataforma que une dos conceptos, educación y foro de ayudas, todo sobre el ámbito de la ciberseguridad. 
Por la parte educacional va a ser muy similar a Udemy/Coderhouse, con módulos iterativos para profesores y alumnos (exámenes, tareas, etc). 
Luego, por la parte de foros de ayudas va a ser muy similar a StackOverflow con el agregado que vas a poder contactar a personas para que te ayuden en vivo.
Como funciona esto? Bien, vos simplemente comentas tu problema y luego te contacta una persona (ayudante experto/amateur de ciberseguridad) para resolverlo, creando sesiones con Jitsi.

Tengo un front con react y 3 microservicios/apis con .NET 8.0
La cual una es una gateway que conecta con las otras dos (Una de foros y la otra educacional). 
La idea del mismo es manejar la autencicacion y autorizacion, entre otros.

## Needs
- Necesito que me ayudes a pensar bien la parte de solicitudes de ayuda. Yo tengo actualmente 7 tablas relacionado con esto. Tengo una clase llamada RequestHelpResponse. Dentro de esta
acabo de agregar una propiedad "RequestHelpTimeSlot TimeSlot" en donde voy a devolver la lista de horarios disponibles de la persona SIEMPRE y cuando ese horario sea mayor al actual, dividido
en franjas de 15 o 30 minutos estrictamente. Esto se puede sacar de la tabla SolicitudAyudaDiponibilidad. Recorda y mirá como utilizo los repositorios con un genericRepository, inyeccion de dependencias,
arquitectura en cebolla, etc...