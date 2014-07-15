param(
	[string]$UserID
)

if ($UserID.length -gt 0)
{
	$credential = Get-Credential
	$so = New-PSSessionOption -SkipCACheck:$true -SkipCNCheck:$true -SkipRevocationCheck:$true
	$session = New-PSSession -ConnectionUri 'https://iolncfevs01/OcsPowershell' -Credential $credential -SessionOption $so
	Import-PSSession $session
	
	Enable-CsUser -Identity $UserID -RegistrarPool 'iolyncpool.ncu.local' -SipAddressType 'emailaddress'
}