using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Request;

namespace apiPB.Validation
{
    public static class MostepDtoValidation
    {
        // Validazione per richiesta Job
        public static MostepRequestDto ValidateJobRequestDto(this MostepRequestDto requestDto)
        {
            GenericValidation.ValidateOnlyAllowedProperties(requestDto, nameof(requestDto.Job));
            return requestDto;
        }

        // Validazione per richiesta Mono che include Job
        public static MostepRequestDto ValidateMonoRequestDto(this MostepRequestDto requestDto)
        {   
            if (string.IsNullOrEmpty(requestDto.Mono))
            {
                throw new ArgumentException("Mono cannot be null or empty.");
            }
            
            if (requestDto.CreationDate == null)
            {
                throw new ArgumentException("CreationDate cannot be null.");
            }
            
            GenericValidation.ValidateOnlyAllowedProperties(requestDto, nameof(requestDto.Job), nameof(requestDto.Mono), nameof(requestDto.CreationDate));
            return requestDto;
        }

        // Validazione per richiesta Operation che include Job e Mono
        public static MostepRequestDto ValidateOperationRequestDto(this MostepRequestDto requestDto)
        {   
            if (string.IsNullOrEmpty(requestDto.Operation))
            {
                throw new ArgumentException("Operation cannot be null or empty.");
            }
            
            GenericValidation.ValidateOnlyAllowedProperties(requestDto, nameof(requestDto.Job), nameof(requestDto.Mono), nameof(requestDto.CreationDate), nameof(requestDto.Operation));
            return requestDto;
        }
    }
}