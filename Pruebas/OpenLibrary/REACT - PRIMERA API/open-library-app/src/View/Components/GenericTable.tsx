import { useEffect, useState } from 'react'

import type Book from '../../Domain/Model/Book'

type Header = {
    key: string,
    label: string
}

interface GenericTableProps {
    headers: Header[],
    data: any[]
}

const formatValue = (value: any) => {
    
    if (typeof value === 'object' && value !== null) {
        return JSON.stringify(value);
    }
    return value;
}

const GenericTable: React.FC<GenericTableProps> = ({ headers, data }) => {
    return (

        <table>
            <thead>
                <tr>
                    {headers.map((header) => (
                        <th key={header.key}>{header.label}</th>
                    ))}
                </tr>
            </thead>
            <tbody>
                {data.map((item, index) => (
                    <tr key={index}>
                        {headers.map((header) => (
                            <td key={header.key}>{formatValue((item as Book)[header.key as keyof Book])}</td>
                        ))}
                    </tr>
                ))}
            </tbody>
        </table>   
    )
}

export default GenericTable